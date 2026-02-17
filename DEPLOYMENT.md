# Deployment Guide

## Architecture Overview

```
Internet --> nginx (ports 80/443, TLS termination)
                |-- static files (Angular SPA)
                |-- /api/* --> reverse proxy --> backend (port 8080, internal)
                                                    |-- PostgreSQL (port 5432, internal)
pgAdmin (port 5050, localhost only -- access via SSH tunnel)
```

- **nginx** serves the Angular SPA and reverse-proxies API requests to the .NET backend. It handles TLS termination using Let's Encrypt certificates.
- **backend** is the .NET 10 API. It runs on HTTP internally (port 8080) since nginx handles HTTPS.
- **db** is PostgreSQL with a Docker volume for data persistence.
- **pgAdmin** is bound to `127.0.0.1:5050` -- only accessible from the server itself (via SSH tunnel).
- **certbot** is a utility service used by the init and renewal scripts to manage Let's Encrypt certificates.
- **migrate** runs database migrations as a one-shot container before the backend starts.

---

## 1. GitHub Repository Setup

### Required Secrets

Go to your repository **Settings > Secrets and variables > Actions** and add:

| Secret | Description | Used In |
|--------|-------------|---------|
| `DOCKERHUB_USERNAME` | Your DockerHub account username | GitHub Actions -- tags and pushes Docker images |
| `DOCKERHUB_TOKEN` | DockerHub access token ([create one here](https://hub.docker.com/settings/security)) | GitHub Actions -- authenticates with DockerHub |

### How It Works

On every push to `main` (or manual trigger), the GitHub Actions workflow (`.github/workflows/build-and-push.yml`):

1. Runs backend tests (`dotnet test`) and frontend tests (`npm test`) in parallel
2. If tests pass, builds Docker images for both backend and frontend
3. Pushes images to DockerHub tagged with `latest` and the short commit SHA

Images are published as:
- `<DOCKERHUB_USERNAME>/angularnetbase-backend:latest`
- `<DOCKERHUB_USERNAME>/angularnetbase-frontend:latest`

---

## 2. Production Server Setup

### Prerequisites

Install on your server:
- **Docker** and **Docker Compose** (v2+)
- **Git** (to clone the repository)
- **curl** and **openssl** (for initial certificate setup)

### DNS Requirements

Before starting, ensure **both** DNS records point to your server's IP:
- `yourdomain.com` (A record)
- `www.yourdomain.com` (A record or CNAME)

Both are required because the SSL certificate covers both domains.

### Initial Setup Steps

#### 2.1 Clone the repository

```bash
git clone <your-repo-url> /opt/angularnetbase
cd /opt/angularnetbase/ops
```

#### 2.2 Create the environment file

```bash
cp .env.example .env
nano .env   # Edit with your actual values
```

Fill in all values. See the [Environment Variables Reference](#3-environment-variables-reference) below.

#### 2.3 Obtain SSL certificates

**Prerequisite**: Docker images must already exist on DockerHub. Push your code to `main` first and wait for the GitHub Actions workflow to complete before proceeding.

Run the initial Let's Encrypt setup (one-time):

```bash
chmod +x scripts/init-letsencrypt.sh scripts/renew-certs.sh
./scripts/init-letsencrypt.sh
```

This script:
1. Creates a temporary self-signed certificate so nginx can start
2. Starts nginx to serve the ACME challenge
3. Requests a real certificate from Let's Encrypt
4. Reloads nginx with the valid certificate

#### 2.4 Start the full stack

```bash
docker compose up -d
```

This starts: database, migration (runs and exits), backend, frontend/nginx, and pgAdmin.

#### 2.5 Set up automatic certificate renewal

Add a cron job that attempts renewal and reloads nginx:

```bash
crontab -e
```

Add this line:

```
0 0,12 * * * /opt/angularnetbase/ops/scripts/renew-certs.sh >> /var/log/certbot-renew.log 2>&1
```

This runs twice daily. Certbot only actually renews certificates that are within 30 days of expiry.

---

## 3. Environment Variables Reference

All variables are set in `ops/.env` (copied from `.env.example`).

| Variable | Example Value | Used By | Purpose |
|----------|---------------|---------|---------|
| `DOMAIN` | `myapp.example.com` | nginx, init-letsencrypt.sh | Domain name for SSL certificates and nginx server_name |
| `DOCKERHUB_USERNAME` | `johndoe` | docker-compose.yml | Prefix for Docker image names |
| `IMAGE_TAG` | `latest` | docker-compose.yml | Docker image tag to pull (default: `latest`) |
| `POSTGRES_DB` | `angularnetbase` | db | PostgreSQL database name |
| `POSTGRES_USER` | `postgres` | db | PostgreSQL username |
| `POSTGRES_PASSWORD` | `(strong password)` | db | PostgreSQL password |
| `CONNECTION_STRING` | `Host=db;Port=5432;...` | backend, migrate | Full Npgsql connection string. Must match the PostgreSQL values above. |
| `JWT_SECRET` | `(32+ byte string)` | backend, migrate | JWT signing key. Must be at least 32 bytes. |
| `JWT_ISSUER` | `AngularNetBase` | backend, migrate | JWT token issuer claim |
| `JWT_AUDIENCE` | `AngularNetBase` | backend, migrate | JWT token audience claim |
| `JWT_ACCESS_TOKEN_EXPIRATION_MINUTES` | `60` | backend | Access token lifetime (default: 60) |
| `JWT_REFRESH_TOKEN_EXPIRATION_DAYS` | `7` | backend | Refresh token lifetime (default: 7) |
| `PGADMIN_DEFAULT_EMAIL` | `admin@example.com` | pgadmin | pgAdmin login email |
| `PGADMIN_DEFAULT_PASSWORD` | `(password)` | pgadmin | pgAdmin login password |
| `LETSENCRYPT_EMAIL` | `admin@example.com` | init-letsencrypt.sh | Email for Let's Encrypt expiry notifications |

### How Environment Variables Flow

```
.env file (on server, never committed to git)
  --> docker-compose.yml reads ${VARIABLE} syntax
    --> Container environment variables:
        .NET backend: Jwt__Secret --> config key Jwt:Secret
                      (double underscore = colon in .NET config hierarchy)
        nginx:        envsubst replaces ${DOMAIN} in config template at startup
        PostgreSQL:   POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD
        pgAdmin:      PGADMIN_DEFAULT_EMAIL, PGADMIN_DEFAULT_PASSWORD
```

---

## 4. Accessing pgAdmin

pgAdmin is bound to `127.0.0.1:5050` on the server -- it is **not** exposed to the internet. To access it, create an SSH tunnel from your local machine.

### Step 1: Open the SSH tunnel

```bash
ssh -L 5050:localhost:5050 user@your-server-ip
```

### Step 2: Open pgAdmin

In your browser, go to:

```
http://localhost:5050
```

Log in with the `PGADMIN_DEFAULT_EMAIL` and `PGADMIN_DEFAULT_PASSWORD` from your `.env` file.

### Step 3: Register the database server

In pgAdmin, click **Add New Server** and use these connection settings:

| Field | Value |
|-------|-------|
| Name | (any descriptive name) |
| Host | `db` |
| Port | `5432` |
| Database | Value of `POSTGRES_DB` from `.env` |
| Username | Value of `POSTGRES_USER` from `.env` |
| Password | Value of `POSTGRES_PASSWORD` from `.env` |

The hostname `db` works because pgAdmin and PostgreSQL are on the same Docker network.

---

## 5. Common Operations

All commands assume you are in the `ops/` directory.

### Deploying a new version

After pushing to `main`, GitHub Actions builds and pushes new images. On the server:

```bash
docker compose pull
docker compose up -d
```

The `migrate` service automatically runs any pending migrations before the backend starts.

### Viewing logs

```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f backend
```

### Restarting a service

```bash
docker compose restart backend
```

### Database backup

```bash
docker compose exec db pg_dump -U postgres angularnetbase > backup_$(date +%Y%m%d).sql
```

### Database restore

```bash
docker compose exec -T db psql -U postgres angularnetbase < backup_file.sql
```

---

## 6. File Reference

| File | Purpose |
|------|---------|
| `ops/docker-compose.yml` | Service definitions (db, migrate, backend, frontend, certbot, pgadmin) |
| `ops/.env.example` | Template for environment variables |
| `ops/nginx.conf` | nginx config template (SSL, reverse proxy, SPA routing) |
| `ops/scripts/init-letsencrypt.sh` | One-time SSL certificate setup |
| `ops/scripts/renew-certs.sh` | Certificate renewal script (run via cron) |
| `backend/Dockerfile` | Backend Docker image (multi-stage .NET build) |
| `frontend/Dockerfile` | Frontend Docker image (Angular build + nginx) |
| `backend/AngularNetBase.API/appsettings.Production.json` | Production config (no secrets -- env vars override at runtime) |
| `.github/workflows/build-and-push.yml` | CI/CD pipeline (test, build, push to DockerHub) |
| `.dockerignore` | Excludes unnecessary files from Docker build context |
