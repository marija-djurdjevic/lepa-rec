#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$PROJECT_DIR"

# Load environment variables
set -a
source .env
set +a

# Attempt renewal (certbot only renews if within 30 days of expiry)
docker compose run --rm certbot renew --quiet

# Reload nginx to pick up any renewed certificates (tolerate failure if nginx is down)
docker compose exec -T frontend nginx -s reload || true

echo "[$(date)] Certificate renewal check completed."
