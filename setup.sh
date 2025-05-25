#!/bin/bash

set -e

DOMAIN="api.nonuso.com"
EMAIL="vmaltarello@gmail.com"

echo "🚀 Avvio setup del server per $DOMAIN"

# 1. Aggiorna pacchetti
sudo apt update && sudo apt upgrade -y

# 2. Installa Docker e Docker Compose se non presenti
if ! command -v docker &> /dev/null; then
  echo "🔧 Installazione Docker..."
  curl -fsSL https://get.docker.com | bash
fi

if ! command -v docker-compose &> /dev/null; then
  echo "🔧 Installazione Docker Compose..."
  sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
  sudo chmod +x /usr/local/bin/docker-compose
fi

# 3. Crea le directory necessarie
echo "📁 Creazione directory certbot se non esiste..."
mkdir -p certbot nginx

# 4. Chiede conferma dominio DNS
echo "⚠️ Assicurati che il dominio $DOMAIN punti all'IP pubblico di questa macchina!"
read -p "Hai impostato correttamente il DNS? (y/n): " confirm
if [[ "$confirm" != "y" ]]; then
  echo "⛔ Interrotto. Sistema il DNS prima di continuare."
  exit 1
fi

# 5. Avvia i container (escl. certbot)
echo "🐳 Avvio container backend..."
docker compose up --build -d

# 6. Genera certificato con Certbot
echo "🔐 Richiesta certificato SSL per $DOMAIN..."
docker compose run --rm certbot certonly --webroot \
  --webroot-path=/var/lib/letsencrypt \
  --email $EMAIL \
  --agree-tos \
  --no-eff-email \
  -d $DOMAIN

# 7. Riavvia nginx con certificato attivo
echo "🔁 Riavvio nginx con certificato SSL..."
docker compose restart nginx

echo "✅ Tutto pronto! API disponibili su: https://$DOMAIN"
