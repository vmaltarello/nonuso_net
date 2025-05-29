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

# Funzione per il logging
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR: $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING: $1${NC}"
}

# =============================================
# 1. Aggiornamento Sistema e Installazione Pacchetti Base
# =============================================
log "Aggiornamento del sistema e installazione pacchetti base..."

# Aggiorna il sistema
sudo apt update && sudo apt upgrade -y

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
    python3-certbot-nginx

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
maxretry = 3
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
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Installa Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

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
user nginx;
worker_processes auto;
worker_cpu_affinity auto;
worker_rlimit_nofile 65535;

error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 4096;
    multi_accept on;
    use epoll;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    # Logging
    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';

    access_log /var/log/nginx/access.log main buffer=512k flush=1m;

    # Basic Settings
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    server_tokens off;
    client_max_body_size 50M;
    client_body_buffer_size 128k;
    client_header_buffer_size 1k;
    large_client_header_buffers 4 4k;
    output_buffers 1 32k;
    postpone_output 1460;
    client_header_timeout 3m;
    client_body_timeout 3m;
    send_timeout 3m;

    # Gzip Settings
    gzip on;
    gzip_disable "msie6";
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_buffers 16 8k;
    gzip_http_version 1.1;
    gzip_min_length 256;
    gzip_types
        application/atom+xml
        application/javascript
        application/json
        application/ld+json
        application/manifest+json
        application/rss+xml
        application/vnd.geo+json
        application/vnd.ms-fontobject
        application/x-font-ttf
        application/x-web-app-manifest+json
        application/xhtml+xml
        application/xml
        font/opentype
        image/bmp
        image/svg+xml
        image/x-icon
        text/cache-manifest
        text/css
        text/plain
        text/vcard
        text/vnd.rim.location.xloc
        text/vtt
        text/x-component
        text/x-cross-domain-policy;

    # Rate Limiting
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=login_limit:10m rate=5r/s;

    # Security Headers
    map $http_upgrade $connection_upgrade {
        default upgrade;
        '' close;
    }

    # SSL Settings
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers on;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_session_cache shared:SSL:50m;
    ssl_session_timeout 1d;
    ssl_session_tickets off;
    ssl_stapling on;
    ssl_stapling_verify on;
    resolver 8.8.8.8 8.8.4.4 valid=300s;
    resolver_timeout 5s;

    # Include virtual host configs
    include /etc/nginx/conf.d/*.conf;
} 
EOL

sudo tee /etc/nginx/conf.d/api.nonuso.com.conf > /dev/null << 'EOL'
upstream api_backend {
    server api:8080;
    keepalive 32;
    keepalive_requests 100;
    keepalive_timeout 60s;
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    listen [::]:80;
    server_name api.nonuso.com;
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;
    
    # Redirect all HTTP traffic to HTTPS
    return 301 https://$server_name$request_uri;
}

# HTTPS server
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name api.nonuso.com;

    # SSL configuration
    ssl_certificate /etc/nginx/ssl/api.nonuso.com.crt;
    ssl_certificate_key /etc/nginx/ssl/api.nonuso.com.key;
    ssl_session_timeout 1d;
    ssl_session_cache shared:SSL:50m;
    ssl_session_tickets off;

    # Modern configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    # HSTS
    add_header Strict-Transport-Security "max-age=63072000" always;

    # OCSP Stapling
    ssl_stapling on;
    ssl_stapling_verify on;
    resolver 8.8.8.8 8.8.4.4 valid=300s;
    resolver_timeout 5s;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' http: https: data: blob: 'unsafe-inline'" always;
    add_header Permissions-Policy "geolocation=(), microphone=(), camera=()" always;

    # Logging
    access_log /var/log/nginx/api.nonuso.com.access.log combined buffer=512k flush=1m;
    error_log /var/log/nginx/api.nonuso.com.error.log warn;

    # API endpoints
    location / {
        # Rate limiting
        limit_req zone=api_limit burst=20 nodelay;
        
        # Proxy settings
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;

        # Buffer size
        proxy_buffer_size 4k;
        proxy_buffers 4 32k;
        proxy_busy_buffers_size 64k;
        proxy_temp_file_write_size 64k;

        # Security
        proxy_hide_header X-Powered-By;
        proxy_hide_header X-AspNet-Version;
        proxy_hide_header X-AspNetMvc-Version;
    }

    # Health check endpoint
    location /health {
        access_log off;
        add_header Content-Type application/json;
        return 200 '{"status":"UP"}';
    }

    # Deny access to hidden files
    location ~ /\. {
        deny all;
        access_log off;
        log_not_found off;
    }

    # Deny access to backup files
    location ~ ~$ {
        deny all;
        access_log off;
        log_not_found off;
    }

    # Deny access to sensitive files
    location ~* \.(git|env|config|ini|log|sql|bak|swp|old)$ {
        deny all;
        access_log off;
        log_not_found off;
    }

    # CORS configuration
    location /api/ {
        if ($request_method = 'OPTIONS') {
            add_header 'Access-Control-Allow-Origin' '*';
            add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS, PUT, DELETE';
            add_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization';
            add_header 'Access-Control-Max-Age' 1728000;
            add_header 'Content-Type' 'text/plain; charset=utf-8';
            add_header 'Content-Length' 0;
            return 204;
        }
        if ($request_method = 'POST') {
            add_header 'Access-Control-Allow-Origin' '*';
            add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS, PUT, DELETE';
            add_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization';
            add_header 'Access-Control-Expose-Headers' 'Content-Length,Content-Range';
        }
        if ($request_method = 'GET') {
            add_header 'Access-Control-Allow-Origin' '*';
            add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS, PUT, DELETE';
            add_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization';
            add_header 'Access-Control-Expose-Headers' 'Content-Length,Content-Range';
        }
    }
} 
EOL

sudo tee /etc/nginx/modsecurity/modsecurity.conf > /dev/null << 'EOL'
# ModSecurity configuration
SecRuleEngine On
SecRequestBodyAccess On
SecRule REQUEST_HEADERS:Content-Type "text/xml" \
     "id:'200000',phase:1,t:none,t:lowercase,pass,nolog,ctl:requestBodyProcessor=XML"

# Basic rule set
SecRule REQUEST_HEADERS:User-Agent "^$" \
    "id:1000,phase:1,deny,status:403,msg:'Empty User Agent'"

SecRule REQUEST_HEADERS:Content-Length "!^[0-9]+$" \
    "id:1001,phase:1,deny,status:403,msg:'Invalid Content Length'"

# SQL Injection Protection
SecRule ARGS|ARGS_NAMES|REQUEST_COOKIES|REQUEST_COOKIES_NAMES|REQUEST_HEADERS|XML:/* "(?i:(\%27)|(\')|(\-\-)|(\%23)|(#))" \
    "id:1002,phase:2,deny,status:403,msg:'SQL Injection Attack'"

# XSS Protection
SecRule ARGS|ARGS_NAMES|REQUEST_COOKIES|REQUEST_COOKIES_NAMES|REQUEST_HEADERS|XML:/* "(?i:(<script|javascript:|<iframe|<object|data:))" \
    "id:1003,phase:2,deny,status:403,msg:'XSS Attack'"

# Path Traversal Protection
SecRule ARGS|ARGS_NAMES|REQUEST_COOKIES|REQUEST_COOKIES_NAMES|REQUEST_HEADERS|XML:/* "(?i:(\.\.\/|\.\.\\|\.\.\/\.\.|\.\.\\\.\.))" \
    "id:1004,phase:2,deny,status:403,msg:'Path Traversal Attack'"

# Command Injection Protection
SecRule ARGS|ARGS_NAMES|REQUEST_COOKIES|REQUEST_COOKIES_NAMES|REQUEST_HEADERS|XML:/* "(?i:(;|\||``|>|<|&|\\|\/))" \
    "id:1005,phase:2,deny,status:403,msg:'Command Injection Attack'"

# File Upload Protection
SecRule FILES|FILES_NAMES "\.(php|phtml|php3|php4|php5|php7|phar|inc|pl|py|jsp|asp|htm|html|shtml|sh|cgi)$" \
    "id:1006,phase:2,deny,status:403,msg:'Invalid File Upload'"

# Rate Limiting
SecRule &ARGS:username "@gt 0" \
    "id:1007,phase:2,pass,nolog,setvar:user.attempt=+1,expirevar:user.attempt=60"

SecRule &ARGS:username "@gt 0" \
    "id:1008,phase:2,deny,status:429,msg:'Too Many Requests',chain"
SecRule &user.attempt "@gt 5" \
    "t:none"

# Logging
SecAuditEngine On
SecAuditLog /var/log/nginx/modsec_audit.log
SecAuditLogParts ABCFHZ
SecAuditLogType Serial
SecAuditLogStorageDir /var/log/nginx/modsec_audit/ 
EOL

# Genera certificati SSL temporanei (saranno sostituiti da Let's Encrypt)
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/ssl/api.nonuso.com.key \
    -out /etc/nginx/ssl/api.nonuso.com.crt \
    -subj "/C=IT/ST=State/L=City/O=Organization/CN=api.nonuso.com"

# Verifica configurazione Nginx
sudo nginx -t

# Riavvia Nginx
sudo systemctl restart nginx

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
  scrape_interval: 15s
  evaluation_interval: 15s

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