#!/bin/bash

# =============================================
# Nonuso.net Server Setup Script
# Questo script configura un nuovo server VPS per Nonuso.net
# =============================================

# Colori per output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Funzioni di logging
log() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
    exit 1
}

warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

# Verifica se lo script è eseguito come root
if [ "$EUID" -ne 0 ]; then 
    error "Questo script deve essere eseguito come root"
fi

# Crea utente non-root per l'applicazione
log "Creazione utente per l'applicazione..."
APP_USER="unonuso"
APP_GROUP="unonuso"

# Crea gruppo se non esiste
if ! getent group $APP_GROUP > /dev/null; then
    groupadd $APP_GROUP
fi

# Crea utente se non esiste
if ! id "$APP_USER" &>/dev/null; then
    useradd -m -s /bin/bash -g $APP_GROUP $APP_USER
    # Genera password casuale
    PASSWORD=$(openssl rand -base64 12)
    echo "$APP_USER:$PASSWORD" | chpasswd
    log "Password generata per $APP_USER: $PASSWORD"
    log "Per favore, cambia la password al primo accesso"
fi

# Aggiungi l'utente ai gruppi necessari
usermod -aG sudo,docker $APP_USER

# Configura SSH per l'utente
log "Configurazione SSH..."
mkdir -p /home/$APP_USER/.ssh
chmod 700 /home/$APP_USER/.ssh
touch /home/$APP_USER/.ssh/authorized_keys
chmod 600 /home/$APP_USER/.ssh/authorized_keys
chown -R $APP_USER:$APP_GROUP /home/$APP_USER/.ssh

# =============================================
# 1. Aggiornamento Sistema e Installazione Pacchetti Base
# =============================================
log "Aggiornamento del sistema e installazione pacchetti base..."

# Aggiorna il sistema
sudo apt update && sudo apt upgrade -y || error "Errore durante l'aggiornamento del sistema"

# Installa pacchetti essenziali
sudo apt install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    software-properties-common \
    gnupg \
    lsb-release \
    fail2ban \
    ufw \
    unattended-upgrades \
    logwatch \
    prometheus \
    prometheus-node-exporter \
    prometheus-pushgateway \
    grafana \
    certbot \
    python3-certbot-nginx \
    git \
    htop \
    net-tools \
    vim \
    unzip \
    zip \
    nginx \
    nodejs \
    npm

# =============================================
# 2. Configurazione Sicurezza Base
# =============================================
log "Configurazione della sicurezza base..."

# Configura automatic security updates
sudo dpkg-reconfigure -plow unattended-upgrades

# Configura UFW (Firewall)
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow 22/tcp  # SSH
sudo ufw allow 80/tcp  # HTTP
sudo ufw allow 443/tcp # HTTPS
sudo ufw enable

# Configura Fail2Ban
sudo tee /etc/fail2ban/jail.local > /dev/null << EOL
[sshd]
enabled = true
port = ssh
filter = sshd
logpath = /var/log/auth.log
maxretry = 5
findtime = 300
bantime = 3600

[nginx-http-auth]
enabled = true
filter = nginx-http-auth
port = http,https
logpath = /var/log/nginx/error.log

[nginx-botsearch]
enabled = true
filter = nginx-botsearch
port = http,https
logpath = /var/log/nginx/access.log
EOL

sudo systemctl restart fail2ban

# =============================================
# 3. Installazione Docker e Docker Compose
# =============================================
log "Installazione di Docker e Docker Compose..."

# Installa Docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Aggiungi l'utente al gruppo docker
sudo usermod -aG docker $USER

# =============================================
# 4. Configurazione Nginx
# =============================================
log "Configurazione di Nginx..."

# Crea directory necessarie
sudo mkdir -p /etc/nginx/modsecurity
sudo mkdir -p /var/log/nginx/modsec_audit
sudo mkdir -p /etc/nginx/ssl

# Installa ModSecurity
sudo apt install -y libnginx-mod-http-modsecurity

# Copia configurazioni Nginx
sudo tee /etc/nginx/nginx.conf > /dev/null << 'EOL'
user www-data;
worker_processes auto;
pid /run/nginx.pid;
include /etc/nginx/modules-enabled/*.conf;

events {
    worker_connections 1024;
    multi_accept on;
}

http {
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    server_tokens off;

    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers on;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;

    access_log /var/log/nginx/access.log;
    error_log /var/log/nginx/error.log;

    gzip on;
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types text/plain text/css text/xml application/json application/javascript application/xml+rss application/atom+xml image/svg+xml;

    include /etc/nginx/conf.d/*.conf;
}
EOL

# =============================================
# 5. Configurazione Let's Encrypt
# =============================================
log "Configurazione di Let's Encrypt..."

# Ottieni certificato SSL
sudo certbot --nginx -d api.nonuso.com --non-interactive --agree-tos --email vmaltarello@gmail.com

# Configura rinnovo automatico
echo "0 0 * * * root certbot renew --quiet" | sudo tee -a /etc/crontab > /dev/null

# =============================================
# 6. Configurazione Monitoring
# =============================================
log "Configurazione del monitoring..."

# Configura Prometheus
sudo tee /etc/prometheus/prometheus.yml > /dev/null << EOL
global:
  scrape_interval: 30s
  evaluation_interval: 30s

scrape_configs:
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']

  - job_name: 'node'
    static_configs:
      - targets: ['localhost:9100']

  - job_name: 'docker'
    static_configs:
      - targets: ['localhost:9323']

  - job_name: 'nonuso-api'
    static_configs:
      - targets: ['api:8080']
EOL

# Configura Grafana
sudo tee /etc/grafana/grafana.ini > /dev/null << EOL
[server]
http_port = 3000
domain = localhost
root_url = http://localhost:3000/

[security]
admin_user = admin
admin_password = your_secure_password

[auth.anonymous]
enabled = true
EOL

# Riavvia servizi
sudo systemctl restart prometheus
sudo systemctl restart grafana-server

# =============================================
# 7. Configurazione Backup
# =============================================
log "Configurazione del sistema di backup..."

# Crea script di backup
sudo tee /opt/nonuso-net/backup.sh > /dev/null << 'EOL'
#!/bin/bash

BACKUP_DIR="/opt/nonuso-backups/$(date +%Y%m%d_%H%M%S)"
mkdir -p $BACKUP_DIR

# Backup del database
docker exec nonuso-postgres pg_dump -U nonusouservm NonusoApp > $BACKUP_DIR/db_backup.sql

# Backup dei volumi Docker
docker run --rm \
  -v nonuso-postgres-data:/source:ro \
  -v $BACKUP_DIR:/backup \
  alpine tar czf /backup/postgres-data.tar.gz -C /source .

docker run --rm \
  -v nonuso-redis-data:/source:ro \
  -v $BACKUP_DIR:/backup \
  alpine tar czf /backup/redis-data.tar.gz -C /source .

# Mantieni solo gli ultimi 7 backup
find /opt/nonuso-backups -type d -mtime +7 -exec rm -rf {} \;
EOL

# Rendi lo script eseguibile
sudo chmod +x /opt/nonuso-net/backup.sh

# Aggiungi al crontab
echo "0 2 * * * /opt/nonuso-net/backup.sh" | sudo tee -a /etc/crontab > /dev/null

# =============================================
# 8. Configurazione Log Rotation
# =============================================
log "Configurazione della rotazione dei log..."

sudo tee /etc/logrotate.d/nonuso > /dev/null << EOL
/var/log/nginx/*.log {
    daily
    missingok
    rotate 14
    compress
    delaycompress
    notifempty
    create 0640 www-data adm
    sharedscripts
    postrotate
        [ -f /var/run/nginx.pid ] && kill -USR1 \`cat /var/run/nginx.pid\`
    endscript
}

/var/log/nginx/modsec_audit.log {
    daily
    missingok
    rotate 14
    compress
    delaycompress
    notifempty
    create 0640 www-data adm
}
EOL

# =============================================
# 9. Configurazione Directory e Permessi
# =============================================
log "Configurazione delle directory e permessi..."

# Crea directory necessarie
sudo mkdir -p /opt/nonuso-net
sudo mkdir -p /opt/nonuso-backups
sudo mkdir -p /var/log/nginx/modsec_audit

# Imposta permessi
sudo chown -R $USER:$USER /opt/nonuso-net
sudo chmod -R 755 /opt/nonuso-net
sudo chown -R www-data:www-data /var/log/nginx

# =============================================
# 10. Verifica Finale
# =============================================
log "Esecuzione verifica finale..."

# Verifica servizi
sudo systemctl status nginx
sudo systemctl status fail2ban
sudo systemctl status prometheus
sudo systemctl status grafana-server

# Verifica configurazione Nginx
sudo nginx -t

# Verifica certificati SSL
sudo certbot certificates

log "Setup completato! Ricordati di:"
log "1. Configurare il DNS per api.nonuso.com"
log "2. Aggiornare le password di default"
log "3. Configurare i backup"
log "4. Verificare i log per eventuali errori"
log "5. Configurare le notifiche di monitoring"

log "Il server è pronto per il deployment!" 