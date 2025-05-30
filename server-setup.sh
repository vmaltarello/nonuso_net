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

# Configura SSH per permettere sia chiavi che password inizialmente
sed -i 's/PasswordAuthentication no/PasswordAuthentication yes/' /etc/ssh/sshd_config
sed -i 's/#PubkeyAuthentication yes/PubkeyAuthentication yes/' /etc/ssh/sshd_config
systemctl restart sshd

# Crea directory per l'applicazione con i permessi corretti
log "Configurazione directory applicazione..."
mkdir -p /home/$APP_USER/nonuso_net
mkdir -p /home/$APP_USER/nonuso_net/secrets
mkdir -p /home/$APP_USER/nonuso_net/nginx
mkdir -p /home/$APP_USER/nonuso_net/logs
chown -R $APP_USER:$APP_GROUP /home/$APP_USER/nonuso_net
chmod -R 755 /home/$APP_USER/nonuso_net
chmod 700 /home/$APP_USER/nonuso_net/secrets

# Configura sudo per l'utente
log "Configurazione sudo per l'utente..."
echo "$APP_USER ALL=(ALL) NOPASSWD: /usr/bin/systemctl status nginx, /usr/bin/systemctl restart nginx, /usr/bin/systemctl stop nginx, /usr/bin/systemctl start nginx, /usr/bin/docker, /usr/bin/docker-compose" | tee /etc/sudoers.d/$APP_USER
chmod 440 /etc/sudoers.d/$APP_USER

# Aggiungi l'utente ai gruppi necessari
usermod -aG docker,adm $APP_USER

# Configura SSH per GitHub Actions
log "Configurazione SSH per GitHub Actions..."
if [ ! -f "/root/github_actions_key" ]; then
    log "Generazione nuova chiave SSH per GitHub Actions..."
    ssh-keygen -t ed25519 -f /root/github_actions_key -N "" -C "vmaltarello@gmail"
    mv /root/github_actions_key.pub /root/github_actions_key.pub.bak
    cat /root/github_actions_key.pub.bak > /root/github_actions_key.pub
    rm /root/github_actions_key.pub.bak
    chmod 600 /root/github_actions_key
    chmod 644 /root/github_actions_key.pub
    # Mostra la chiave PRIVATA da aggiungere ai secrets di GitHub Actions
    log "Chiave SSH PRIVATA generata. Aggiungi QUESTA chiave a GitHub Actions secrets (VPS_SSH_KEY):"
    echo -e "${YELLOW}"
    cat /root/github_actions_key
    echo -e "${NC}"
fi

# Aggiungi la chiave pubblica all'authorized_keys dell'utente unonuso
if [ -f "/root/github_actions_key.pub" ]; then
    log "Aggiunta chiave pubblica all'utente unonuso..."
    mkdir -p /home/unonuso/.ssh
    cat /root/github_actions_key.pub >> /home/unonuso/.ssh/authorized_keys
    chown -R unonuso:unonuso /home/unonuso/.ssh
    chmod 700 /home/unonuso/.ssh
    chmod 600 /home/unonuso/.ssh/authorized_keys
    log "Chiave pubblica aggiunta e permessi impostati."
fi

log "Setup SSH completato!"
echo -e "\n${YELLOW}IMPORTANTE:${NC}"
echo "1. Per aggiungere una nuova chiave SSH, usa:"
echo "   sudo add-ssh-key 'chiave_pubblica'"
echo "2. Per GitHub Actions, copia la chiave pubblica in:"
echo "   /root/github_actions_key.pub"
echo "3. Dopo aver configurato tutte le chiavi necessarie, puoi disabilitare l'accesso con password:"
echo "   sudo sed -i 's/PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config"
echo "   sudo systemctl restart sshd"
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
# 4. Configurazione Nginx e Let's Encrypt (Sequenza Corretta)
# =============================================
log "Configurazione di Nginx e Let's Encrypt..."

# Crea directory necessarie
sudo mkdir -p /etc/nginx/ssl
sudo mkdir -p /etc/nginx/sites-available
sudo mkdir -p /etc/nginx/sites-enabled

# Copia configurazioni Nginx (incluse le direttive SSL che punteranno ai file che Certbot creerà)
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
    include /etc/nginx/sites-enabled/*;
}
EOL

# Crea configurazione per api.nonuso.com
sudo tee /etc/nginx/sites-available/api.nonuso.com > /dev/null << 'EOL'
server {
    listen 80;
    server_name api.nonuso.com;

    location / {
        # Reindirizza a HTTPS - questo sarà attivo solo dopo che Certbot avrà ottenuto il certificato
        return 301 https://$host$request_uri;
    }
}

server {
    listen 443 ssl http2;
    server_name api.nonuso.com;

    # Questi path verranno creati da Certbot
    ssl_certificate /etc/letsencrypt/live/api.nonuso.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.nonuso.com/privkey.pem;

    # SSL configuration
    ssl_session_timeout 1d;
    ssl_session_cache shared:SSL:50m;
    ssl_session_tickets off;

    # HSTS (uncomment if you're sure)
    # add_header Strict-Transport-Security "max-age=63072000" always;

    # Proxy settings
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
EOL

# Abilita il sito
sudo ln -sf /etc/nginx/sites-available/api.nonuso.com /etc/nginx/sites-enabled/

# FERMA Nginx per permettere a Certbot di usare la porta 80 in modalità standalone
log "Fermando Nginx per Certbot (modalità standalone)..."
sudo systemctl stop nginx || true # Permetti che fallisca se non è già attivo

# Ottieni certificato SSL con Certbot in modalità standalone
# Assumendo che il DNS punti già al server
log "Ottenendo certificato SSL con Certbot (standalone)..."
sudo certbot certonly --standalone -d api.nonuso.com --non-interactive --agree-tos -m vmaltarello@gmail.com \
    || { error "Errore critico: Certificato Let's Encrypt non ottenuto. Controlla il DNS e i log di Certbot (/var/log/letsencrypt/)."; }

# Avvia Nginx (ora i certificati dovrebbero esistere)
log "Avviando Nginx..."
sudo systemctl start nginx \
    || { error "Errore critico: Impossibile avviare Nginx. Controlla i log di Nginx (journalctl -xeu nginx.service)."; }

sudo systemctl enable nginx # Assicura che Nginx parta all'avvio

# Configura rinnovo automatico (Certbot gestisce già il cronjob con l'installazione)
log "Rinnovo automatico Certbot configurato."

log "Configurazione Nginx e Certbot completata!"

# =============================================
# 6. Configurazione Backup
# =============================================
log "Configurazione del sistema di backup..."

# Crea directory per i backup
sudo mkdir -p /opt/nonuso-net
sudo mkdir -p /opt/nonuso-backups

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
# 7. Configurazione Log Rotation
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
# 8. Configurazione Directory e Permessi
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
# 9. Verifica Finale
# =============================================
log "Esecuzione verifica finale..."

# Verifica servizi
sudo systemctl status nginx
sudo systemctl status fail2ban

# Verifica configurazione Nginx
sudo nginx -t

# Verifica certificati SSL
sudo certbot certificates

log "Setup completato! Ricordati di:"
log "1. Configurare il DNS per api.nonuso.com"
log "2. Aggiornare le password di default"
log "3. Configurare i backup"
log "4. Verificare i log per eventuali errori"

log "Il server è pronto per il deployment!" 