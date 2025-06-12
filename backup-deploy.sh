#!/bin/bash
set -euo pipefail

DEPLOY_BACKUP_DIR="/opt/deploy-backups"
DATE=$(date +%Y%m%d_%H%M%S)
APP_DIR="/home/unonuso/nonuso_net"

# Crea backup pre-deploy
mkdir -p "${DEPLOY_BACKUP_DIR}"
cd "${APP_DIR}"

# Backup stato attuale
tar czf "${DEPLOY_BACKUP_DIR}/pre-deploy-${DATE}.tar.gz" \
    --exclude="node_modules" \
    --exclude=".git" \
    --exclude="*.log" \
    .

# Upload su Google Drive
if command -v rclone &> /dev/null && rclone listremotes | grep -q "gdrive:"; then
    rclone mkdir "gdrive:/NonusoApp-VPS1/deploy-backups/$(date +%Y-%m)" || true
    rclone move "${DEPLOY_BACKUP_DIR}/pre-deploy-${DATE}.tar.gz" \
        "gdrive:/NonusoApp-VPS1/deploy-backups/$(date +%Y-%m)/" || true
fi

# Pulizia locale (mantieni solo ultimi 3)
ls -t ${DEPLOY_BACKUP_DIR}/pre-deploy-*.tar.gz | tail -n +4 | xargs rm -f || true

# Pulizia Drive (più vecchi di 7 giorni)
rclone delete "gdrive:/NonusoApp-VPS1/deploy-backups" --min-age 7d || true