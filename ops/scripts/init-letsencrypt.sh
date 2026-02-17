#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$PROJECT_DIR"

# Load environment variables (DOMAIN, LETSENCRYPT_EMAIL, etc.)
set -a
source .env
set +a

DOMAIN="${DOMAIN:?DOMAIN must be set in .env}"
EMAIL="${LETSENCRYPT_EMAIL:?LETSENCRYPT_EMAIL must be set in .env}"
DATA_PATH="./certbot"
RSA_KEY_SIZE=4096

echo "### Setting up Let's Encrypt for $DOMAIN ###"

# Create directories
mkdir -p "$DATA_PATH/conf/live/$DOMAIN"
mkdir -p "$DATA_PATH/www"

# Download recommended TLS parameters if not present
if [ ! -e "$DATA_PATH/conf/options-ssl-nginx.conf" ]; then
  echo "Downloading recommended TLS parameters..."
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf \
    > "$DATA_PATH/conf/options-ssl-nginx.conf"
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem \
    > "$DATA_PATH/conf/ssl-dhparams.pem"
fi

# Create temporary self-signed certificate so nginx can start
echo "Creating temporary self-signed certificate..."
openssl req -x509 -nodes -newkey rsa:$RSA_KEY_SIZE -days 1 \
  -keyout "$DATA_PATH/conf/live/$DOMAIN/privkey.pem" \
  -out "$DATA_PATH/conf/live/$DOMAIN/fullchain.pem" \
  -subj "/CN=localhost"

# Start nginx with the temporary certificate
echo "Starting nginx..."
docker compose up -d frontend

# Wait for nginx to be ready
echo "Waiting for nginx to start..."
sleep 5

# Delete the temporary certificate
rm -rf "$DATA_PATH/conf/live/$DOMAIN"

# Request real certificate from Let's Encrypt
# Note: Requests for both bare domain and www subdomain.
# Both must have DNS records pointing to this server.
echo "Requesting Let's Encrypt certificate for $DOMAIN..."
docker compose run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email "$EMAIL" \
  --agree-tos \
  --no-eff-email \
  -d "$DOMAIN" \
  -d "www.$DOMAIN" \
  --rsa-key-size $RSA_KEY_SIZE \
  --force-renewal

# Reload nginx with the real certificate
echo "Reloading nginx..."
docker compose exec frontend nginx -s reload

echo ""
echo "### SSL certificate obtained successfully for $DOMAIN ###"
echo "You can now start the full stack with:"
echo "  cd $(pwd) && docker compose up -d"
