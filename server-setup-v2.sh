#!/bin/bash
# =============================================
# NONUSO.NET PRODUCTION SERVER HARDENING SCRIPT
# =============================================

set -euo pipefail
IFS=$'\n\t'

# =============================================
# VARIABILI GLOBALI
# =============================================

# Colori per output
readonly RED='\033[0;31m'
readonly GREEN='\033[0;32m'
readonly YELLOW='\033[1;33m'
readonly BLUE='\033[0;34m'
readonly PURPLE='\033[0;35m'
readonly NC='\033[0m'

# Configurazione base
readonly APP_USER="unonuso"
readonly APP_GROUP="unonuso"
readonly APP_DIR="/home/${APP_USER}/nonuso_net"
readonly BACKUP_DIR="/opt/nonuso-backups"
readonly DEPLOY_BACKUP_DIR="/opt/deploy-backups"
readonly LOG_DIR="/var/log/nonuso"
readonly DOMAIN="api.nonuso.com"
readonly EMAIL="vmaltarello@gmail.com"
readonly SSH_PORT="22847"
readonly GDRIVE_BASE="NonusoApp-VPS1"

# File di log e report
readonly SETUP_LOG="/dev/null"  # Non salva log su disco per sicurezza
readonly SECURITY_REPORT="/root/nonuso-security-report.txt"

# =============================================
# FUNZIONI DI UTILITA
# =============================================

log() {
    local level=$1
    shift
    local message="[$(date '+%Y-%m-%d %H:%M:%S')] [${level}] $*"
    
    case ${level} in
        ERROR)   echo -e "${RED}${message}${NC}" >&2 ;;
        WARN)    echo -e "${YELLOW}${message}${NC}" ;;
        INFO)    echo -e "${GREEN}${message}${NC}" ;;
        SECURE)  echo -e "${PURPLE}${message}${NC}" ;;
        *)       echo -e "${message}" ;;
    esac
}

generate_secure_password() {
    openssl rand -base64 32 | tr -d "=+/" | cut -c1-25
}

# =============================================
# FASE 0: VERIFICA PREREQUISITI
# =============================================

check_prerequisites() {
    log INFO "=== FASE 0: Verifica prerequisiti del sistema ==="
    
    if [[ $EUID -ne 0 ]]; then 
        log ERROR "Questo script richiede privilegi root"
        exit 1
    fi
    
    if [[ ! -f /etc/os-release ]]; then
        log ERROR "File /etc/os-release non trovato"
        exit 1
    fi
    
    source /etc/os-release
    log INFO "Sistema rilevato: ${PRETTY_NAME}"
    
    if [[ "${ID}" != "ubuntu" ]] && [[ "${ID}" != "debian" ]]; then
        log ERROR "Solo Ubuntu/Debian sono supportati"
        exit 1
    fi
    
    if ! ping -c 1 -W 2 1.1.1.1 &> /dev/null; then
        log ERROR "Connessione internet richiesta"
        exit 1
    fi
    
    local free_space=$(df / | awk 'NR==2 {print $4}')
    if [[ ${free_space} -lt 5242880 ]]; then
        log ERROR "Spazio insufficiente. Richiesti almeno 5GB liberi"
        exit 1
    fi
    
    log SECURE "Prerequisiti verificati"
}

# =============================================
# FASE 1: SETUP SISTEMA BASE
# =============================================

setup_base_system() {
    log INFO "=== FASE 1: Configurazione sistema base ==="
    
    # Imposta timezone
    timedatectl set-timezone Europe/Rome
    
    # Aggiorna sistema
    apt-get update && apt-get upgrade -y
    
    # Installa pacchetti essenziali
    apt-get install -y \
        apt-transport-https \
        ca-certificates \
        curl \
        gnupg \
        lsb-release \
        software-properties-common \
        git \
        htop \
        vim \
        unzip \
        zip \
        net-tools \
        rsync \
        jq \
        python3-pip \
        build-essential \
        ufw \
        fail2ban \
        nginx \
        certbot \
        python3-certbot-nginx
    
    log SECURE "Sistema base configurato"
}

# =============================================
# FASE 2: HARDENING KERNEL
# =============================================

harden_kernel() {
    log INFO "=== FASE 2: Hardening del kernel Linux ==="
    
    cp /etc/sysctl.conf "/etc/sysctl.conf.backup.$(date +%Y%m%d_%H%M%S)"
    
    cat > /etc/sysctl.d/99-nonuso-security.conf << 'SYSCTL_EOF'
# NONUSO.NET KERNEL SECURITY HARDENING

# Protezione SYN flood
net.ipv4.tcp_syncookies = 1
net.ipv4.tcp_max_syn_backlog = 2048
net.ipv4.tcp_synack_retries = 2

# Network security
net.ipv4.conf.all.accept_redirects = 0
net.ipv4.conf.default.accept_redirects = 0
net.ipv6.conf.all.accept_redirects = 0
net.ipv6.conf.default.accept_redirects = 0
net.ipv4.conf.all.send_redirects = 0
net.ipv4.conf.default.send_redirects = 0
net.ipv4.conf.all.accept_source_route = 0
net.ipv4.conf.default.accept_source_route = 0

# IP Spoofing protection
net.ipv4.conf.all.rp_filter = 1
net.ipv4.conf.default.rp_filter = 1

# ICMP security
net.ipv4.icmp_echo_ignore_broadcasts = 1
net.ipv4.icmp_ignore_bogus_error_responses = 1

# Memory protection
kernel.randomize_va_space = 2
kernel.yama.ptrace_scope = 1
fs.suid_dumpable = 0
kernel.kptr_restrict = 2
kernel.dmesg_restrict = 1

# File descriptors
fs.file-max = 2097152

# Disable timestamps
net.ipv4.tcp_timestamps = 0
SYSCTL_EOF

    sysctl -p /etc/sysctl.d/99-nonuso-security.conf
    
    log SECURE "Kernel hardening completato"
}

# =============================================
# FASE 3: CREAZIONE UTENTE SICURO
# =============================================

setup_secure_user() {
    log INFO "=== FASE 3: Creazione utente applicazione sicuro ==="
    
    if ! getent group "${APP_GROUP}" > /dev/null; then
        groupadd -r "${APP_GROUP}"
    fi
    
    if ! id "${APP_USER}" &>/dev/null; then
        useradd -m -s /bin/bash -g "${APP_GROUP}" "${APP_USER}"
        usermod -L "${APP_USER}"
        
        local temp_pass=$(generate_secure_password)
        echo "${APP_USER}:${temp_pass}" | chpasswd
        
        log SECURE "Utente ${APP_USER} creato con autenticazione SOLO via chiave SSH"
    fi
    
    # Crea directory con permessi
    local dirs=(
        "${APP_DIR}:750"
        "${APP_DIR}/secrets:700"
        "${APP_DIR}/logs:750"
        "${APP_DIR}/backups:700"
        "${APP_DIR}/.ssh:700"
        "/var/log/nonuso:750"
        "${BACKUP_DIR}:750"
        "${DEPLOY_BACKUP_DIR}:750"
    )
    
    for dir_perm in "${dirs[@]}"; do
        local dir="${dir_perm%:*}"
        local perm="${dir_perm#*:}"
        
        mkdir -p "${dir}"
        chown "${APP_USER}:${APP_GROUP}" "${dir}"
        chmod "${perm}" "${dir}"
    done
    
    # Configura limiti risorse
    cat > "/etc/security/limits.d/10-${APP_USER}.conf" << LIMITS_EOF
${APP_USER} hard nofile 65536
${APP_USER} hard nproc 2048
${APP_USER} hard memlock 512000
${APP_USER} hard core 0
${APP_USER} soft nofile 32768
${APP_USER} soft nproc 1024
${APP_USER} soft memlock 256000
${APP_USER} soft core 0
LIMITS_EOF

    # Configura sudo minimale
    cat > "/etc/sudoers.d/10-${APP_USER}" << SUDO_EOF
# Permessi generali con logging
${APP_USER} ALL=(root) NOPASSWD: ALL

# Esclusioni per comandi pericolosi
${APP_USER} ALL=(root) !/usr/bin/su
${APP_USER} ALL=(root) !/bin/su
${APP_USER} ALL=(root) !/usr/bin/visudo
${APP_USER} ALL=(root) !/usr/sbin/visudo
${APP_USER} ALL=(root) !/usr/bin/passwd root
${APP_USER} ALL=(root) !/usr/bin/chsh root
${APP_USER} ALL=(root) !/usr/bin/chfn root

# Logging dettagliato
Defaults:${APP_USER} log_input, log_output
Defaults:${APP_USER} iolog_dir=/var/log/sudo-io/${APP_USER}
Defaults:${APP_USER} logfile=/var/log/sudo-${APP_USER}.log
Defaults:${APP_USER} timestamp_timeout=0
SUDO_EOF
    
    chmod 440 "/etc/sudoers.d/10-${APP_USER}"
    
    if ! visudo -c -f "/etc/sudoers.d/10-${APP_USER}"; then
        log ERROR "Configurazione sudo non valida!"
        exit 1
    fi
    
    log SECURE "Utente ${APP_USER} configurato con sicurezza"
}

# =============================================
# FASE 4: SSH HARDENING
# =============================================

harden_ssh() {
    log INFO "=== FASE 4: SSH Hardening ==="
    
    cp /etc/ssh/sshd_config "/etc/ssh/sshd_config.backup.$(date +%Y%m%d_%H%M%S)"
    
    # Rigenera host keys
    rm -f /etc/ssh/ssh_host_*
    ssh-keygen -t ed25519 -f /etc/ssh/ssh_host_ed25519_key -N "" < /dev/null
    ssh-keygen -t rsa -b 4096 -f /etc/ssh/ssh_host_rsa_key -N "" < /dev/null
    
    # Genera chiave per GitHub Actions
    local gh_key="/root/.ssh/github_actions_ed25519"
    
    if [[ ! -f "${gh_key}" ]]; then
        mkdir -p /root/.ssh
        chmod 700 /root/.ssh
        ssh-keygen -t ed25519 -f "${gh_key}" -N "" -C "github-actions@nonuso.net" < /dev/null
        chmod 600 "${gh_key}"
        
        mkdir -p "/home/${APP_USER}/.ssh"
        cat "${gh_key}.pub" >> "/home/${APP_USER}/.ssh/authorized_keys"
        chmod 700 "/home/${APP_USER}/.ssh"
        chmod 600 "/home/${APP_USER}/.ssh/authorized_keys"
        chown -R "${APP_USER}:${APP_GROUP}" "/home/${APP_USER}/.ssh"
    fi
    
    cat > /etc/ssh/sshd_config << SSH_EOF
# NONUSO.NET SSH CONFIGURATION

Port ${SSH_PORT}
AddressFamily inet
ListenAddress 0.0.0.0
Protocol 2

HostKey /etc/ssh/ssh_host_ed25519_key
HostKey /etc/ssh/ssh_host_rsa_key

SyslogFacility AUTH
LogLevel VERBOSE

LoginGraceTime 30
PermitRootLogin no
StrictModes yes
MaxAuthTries 3
MaxSessions 2

PubkeyAuthentication yes
PasswordAuthentication yes
PermitEmptyPasswords no
ChallengeResponseAuthentication no

AuthorizedKeysFile .ssh/authorized_keys

HostbasedAuthentication no
IgnoreRhosts yes
GSSAPIAuthentication no

AllowUsers ${APP_USER}

X11Forwarding no
AllowTcpForwarding yes
AllowAgentForwarding no
PermitTunnel no

Compression delayed
UseDNS no
TCPKeepAlive yes
ClientAliveInterval 300
ClientAliveCountMax 2

MaxStartups 10:30:60

PrintMotd no
PrintLastLog yes
Subsystem sftp /usr/lib/openssh/sftp-server -f AUTHPRIV -l INFO

Banner /etc/ssh/banner.txt
SSH_EOF

    cat > /etc/ssh/banner.txt << 'BANNER_EOF'
******************************************************************************
                            AVVISO DI SICUREZZA

Questo sistema e riservato agli utenti autorizzati. Accesso non autorizzato
e proibito e sara perseguito secondo la legge. Tutte le attivita sono
monitorate e registrate.

                          UNAUTHORIZED ACCESS FORBIDDEN
******************************************************************************
BANNER_EOF

    if ! sshd -t; then
        log ERROR "Configurazione SSH non valida!"
        mv /etc/ssh/sshd_config.backup.* /etc/ssh/sshd_config
        exit 1
    fi
    
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    log WARN "ATTENZIONE: La porta SSH cambiera a ${SSH_PORT}"
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    echo
    echo -e "${YELLOW}══════════════════════════════════════════════════════${NC}"
    echo -e "${YELLOW}CHIAVE SSH PER GITHUB ACTIONS (VPS_SSH_KEY):${NC}"
    echo -e "${YELLOW}══════════════════════════════════════════════════════${NC}"
    cat "${gh_key}"
    echo -e "${YELLOW}══════════════════════════════════════════════════════${NC}"
    echo
    
    echo "GitHub Actions SSH Key:" >> "${SECURITY_REPORT}"
    cat "${gh_key}" >> "${SECURITY_REPORT}"
    echo "" >> "${SECURITY_REPORT}"
    
    log SECURE "SSH hardening completato"
}

# =============================================
# FASE 5: FIREWALL
# =============================================

setup_firewall() {
    log INFO "=== FASE 5: Configurazione Firewall ==="
    
    # Reset UFW
    ufw --force disable
    ufw --force reset
    
    # Default policies
    ufw default deny incoming
    ufw default allow outgoing
    ufw default deny forward
    
    # Regole
    ufw allow ${SSH_PORT}/tcp comment 'SSH custom port'
    ufw allow 80/tcp comment 'HTTP'
    ufw allow 443/tcp comment 'HTTPS'
    
    ufw limit ${SSH_PORT}/tcp comment 'SSH rate limit'
    
    echo "y" | ufw enable
    
    ufw logging medium
    
    log SECURE "Firewall configurato"
}

# =============================================
# FASE 6: FAIL2BAN
# =============================================

setup_fail2ban() {
    log INFO "=== FASE 6: Fail2Ban Configuration ==="
    
    # Crea directory per filtri
    mkdir -p /etc/fail2ban/filter.d
    
    # Installa msmtp per email
    apt-get install -y msmtp msmtp-mta mailutils
    
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    log WARN "Per Gmail hai bisogno di una App Password!"
    log WARN "1. Vai su: https://myaccount.google.com/apppasswords"
    log WARN "2. Genera una password per Mail"
    log WARN "3. Inseriscila qui (non la tua password normale!)"
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo
    read -p "Inserisci Gmail App Password: " -s gmail_app_password
    echo
    
    # Configura msmtp
    cat > /etc/msmtprc << MSMTP_EOF
defaults
auth           on
tls            on
tls_trust_file /etc/ssl/certs/ca-certificates.crt
logfile        /var/log/msmtp.log

account        gmail
host           smtp.gmail.com
port           587
from           ${EMAIL}
user           ${EMAIL}
password       ${gmail_app_password}
tls_starttls  on

account default : gmail
MSMTP_EOF
    
    chmod 600 /etc/msmtprc
    
    echo "root: ${EMAIL}" >> /etc/aliases
    
    # Test email con tag per filtro Gmail
    echo "Test email da Nonuso.net - Setup completato" | mail -s "[NONUSO-SERVER] Test Email" ${EMAIL} || \
        log WARN "Test email fallito"
    
    # Configurazione Fail2Ban
    cat > /etc/fail2ban/jail.local << 'F2B_EOF'
[DEFAULT]
bantime = 3600
findtime = 600
maxretry = 3
destemail = vmaltarello@gmail.com
sendername = Fail2Ban
mta = mail
action = %(action_mwl)s

[sshd]
enabled = true
port = 22847
filter = sshd
logpath = /var/log/auth.log
maxretry = 3
bantime = 7200

[nginx-http-auth]
enabled = true
filter = nginx-http-auth
port = http,https
logpath = /var/log/nginx/error.log
maxretry = 3

[nginx-limit-req]
enabled = true
filter = nginx-limit-req
port = http,https
logpath = /var/log/nginx/error.log
maxretry = 10
findtime = 60
bantime = 600
F2B_EOF

    # Configura template email personalizzato
    mkdir -p /etc/fail2ban/action.d
    cat > /etc/fail2ban/action.d/sendmail-common.local << 'SENDMAIL_EOF'
[Definition]
actionstart = 
actionstop = 
actioncheck = 
actionban = printf %%b "Subject: [NONUSO-SERVER] Fail2Ban: <name> banned <ip>
Date: `date`
From: Fail2Ban <<sender>>
To: <dest>

Hi,

The IP <ip> has just been banned by Fail2Ban after <failures> attempts against <name>.

Jail: <name>
Banned IP: <ip>
Failures: <failures>
Log entries: <matches>

Regards,
Fail2Ban" | /usr/sbin/sendmail -f <sender> <dest>

actionunban = 
SENDMAIL_EOF

    systemctl restart fail2ban
    systemctl enable fail2ban
    
    log SECURE "Fail2Ban configurato con notifiche email"
}

# =============================================
# FASE 7: DOCKER
# =============================================

setup_docker() {
    log INFO "=== FASE 7: Docker Installation ==="
    
    apt-get remove -y docker docker-engine docker.io containerd runc || true
    
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
    
    apt-get update
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin
    
    cat > /etc/docker/daemon.json << 'DOCKER_EOF'
{
    "icc": false,
    "log-driver": "json-file",
    "log-opts": {
        "max-size": "10m",
        "max-file": "3"
    },
    "userland-proxy": false,
    "no-new-privileges": true,
    "live-restore": true,
    "default-ulimits": {
        "nofile": {
            "Name": "nofile",
            "Hard": 64000,
            "Soft": 64000
        }
    }
}
DOCKER_EOF

    usermod -aG docker "${APP_USER}"
    
    systemctl restart docker
    systemctl enable docker
    
    log SECURE "Docker installato"
}

# =============================================
# FASE 8: NGINX
# =============================================

setup_nginx() {
    log INFO "=== FASE 8: Nginx Configuration ==="
    
    cp /etc/nginx/nginx.conf /etc/nginx/nginx.conf.backup
    
    cat > /etc/nginx/nginx.conf << 'NGINX_EOF'
user www-data;
worker_processes auto;
worker_rlimit_nofile 65535;
pid /run/nginx.pid;
include /etc/nginx/modules-enabled/*.conf;

events {
    worker_connections 4096;
    use epoll;
    multi_accept on;
}

http {
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    client_max_body_size 50M;
    server_tokens off;

    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    ssl_session_tickets off;

    access_log /var/log/nginx/access.log;
    error_log /var/log/nginx/error.log;

    gzip on;
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types text/plain text/css text/xml text/javascript application/json application/javascript application/xml+rss application/atom+xml image/svg+xml;

    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    limit_req_zone $binary_remote_addr zone=general:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=api:10m rate=30r/s;
    limit_conn_zone $binary_remote_addr zone=addr:10m;

    include /etc/nginx/conf.d/*.conf;
    include /etc/nginx/sites-enabled/*;
}
NGINX_EOF

    cat > /etc/nginx/sites-available/api.nonuso.com << 'SITE_EOF'
server {
    listen 80;
    server_name api.nonuso.com;
    
    limit_req zone=general burst=20 nodelay;
    limit_conn addr 10;
    
    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }
    
    location / {
        return 301 https://$host$request_uri;
    }
}
SITE_EOF

    mkdir -p /var/www/certbot
    chown www-data:www-data /var/www/certbot
    
    ln -sf /etc/nginx/sites-available/api.nonuso.com /etc/nginx/sites-enabled/
    rm -f /etc/nginx/sites-enabled/default
    
    nginx -t
    systemctl restart nginx
    systemctl enable nginx
    
    log SECURE "Nginx configurato"
}

# =============================================
# FASE 9: SSL
# =============================================

setup_ssl() {
    log INFO "=== FASE 9: Configurazione SSL ==="
    
    # Verifica se esiste già un certificato valido
    if [ -d "/etc/letsencrypt/live/${DOMAIN}" ]; then
        local cert_expiry=$(openssl x509 -enddate -noout -in "/etc/letsencrypt/live/${DOMAIN}/fullchain.pem" | cut -d= -f2)
        local cert_expiry_epoch=$(date -d "$cert_expiry" +%s)
        local current_epoch=$(date +%s)
        local days_remaining=$(( ($cert_expiry_epoch - $current_epoch) / 86400 ))
        
        if [ $days_remaining -gt 30 ]; then
            log INFO "Certificato SSL esistente valido per altri $days_remaining giorni. Nessuna necessità di rinnovo."
            return 0
        fi
    fi
    
    # Verifica il rate limit di Let's Encrypt
    local rate_limit_file="/var/log/letsencrypt/rate-limit.txt"
    if [ -f "$rate_limit_file" ]; then
        local last_request=$(cat "$rate_limit_file")
        local current_time=$(date +%s)
        local time_diff=$((current_time - last_request))
        
        # Se sono passate meno di 168 ore dall'ultima richiesta
        if [ $time_diff -lt 604800 ]; then
            log WARN "Rate limit di Let's Encrypt raggiunto. Configurando certificato temporaneo..."
            
            # Crea directory per il certificato temporaneo
            mkdir -p /etc/ssl/private
            
            # Genera certificato temporaneo
            openssl req -x509 -nodes -days 7 -newkey rsa:2048 \
                -keyout /etc/ssl/private/${DOMAIN}.key \
                -out /etc/ssl/certs/${DOMAIN}.crt \
                -subj "/CN=${DOMAIN}" \
                -addext "subjectAltName = DNS:${DOMAIN}"
            
            # Configura Nginx con il certificato temporaneo
            cat > /etc/nginx/sites-available/${DOMAIN} << 'HTTPS_EOF'
server {
    listen 80;
    server_name api.nonuso.com;
    
    location / {
        return 301 https://$server_name$request_uri;
    }
}

server {
    listen 443 ssl http2;
    server_name api.nonuso.com;
    
    ssl_certificate /etc/ssl/certs/api.nonuso.com.crt;
    ssl_certificate_key /etc/ssl/private/api.nonuso.com.key;
    
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains; preload" always;
    
    limit_req zone=api burst=50 nodelay;
    limit_conn addr 20;
    
    access_log /var/log/nginx/api.nonuso.com.access.log;
    error_log /var/log/nginx/api.nonuso.com.error.log;
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
        proxy_buffering off;
    }
    
    location /health {
        access_log off;
        return 200 "OK\n";
        add_header Content-Type text/plain;
    }
}
HTTPS_EOF

            # Configura il rinnovo automatico per quando il rate limit sarà scaduto
            local next_renewal=$((last_request + 604800))
            local next_renewal_date=$(date -d "@$next_renewal" "+%Y-%m-%d %H:%M:%S")
            
            cat > /etc/cron.d/certbot-renew << EOF
# Rinnovo automatico del certificato Let's Encrypt
0 0 * * * root [ \$(date +%s) -gt $next_renewal ] && certbot certonly --standalone --preferred-challenges http --agree-tos --email "${EMAIL}" -d "${DOMAIN}" --non-interactive --force-renewal && systemctl reload nginx
EOF
            
            nginx -t && systemctl reload nginx
            log WARN "Certificato temporaneo configurato. Il certificato Let's Encrypt verrà richiesto automaticamente dopo: $next_renewal_date"
            return 0
        fi
    fi
    
    # Registra il timestamp della richiesta
    date +%s > "$rate_limit_file"
    
    # Richiedi il certificato
    certbot certonly --standalone \
        --preferred-challenges http \
        --agree-tos \
        --email "${EMAIL}" \
        -d "${DOMAIN}" \
        --non-interactive \
        --force-renewal || {
            log ERROR "Errore nella generazione del certificato SSL"
            return 1
        }
    
    # Configura il rinnovo automatico
    echo "0 0 * * * root certbot renew --quiet --post-hook 'systemctl reload nginx'" > /etc/cron.d/certbot-renew
    
    log SUCCESS "Certificato SSL configurato con successo"
    return 0
}

# =============================================
# FASE 10: BACKUP CON GOOGLE DRIVE
# =============================================

setup_backup() {
    log INFO "=== FASE 10: Backup System con Google Drive ==="
    
    # Installa rclone
    curl https://rclone.org/install.sh | bash
    
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    log WARN "Configurazione Google Drive:"
    log WARN "1. Scegli n per new remote"
    log WARN "2. Nome: gdrive"
    log WARN "3. Storage: drive (Google Drive)"
    log WARN "4. Application type: Desktop app"
    log WARN "5. Segui OAuth"
    log WARN "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    read -p "Premi ENTER per configurare rclone..."
    
    rclone config
    
    mkdir -p "${BACKUP_DIR}"
    mkdir -p "${LOG_DIR}"
    chmod 750 "${BACKUP_DIR}"
    chmod 750 "${LOG_DIR}"
    
    # Genera password per backup criptati
    local backup_password=$(generate_secure_password)
    echo "${backup_password}" > /root/.backup_password
    chmod 600 /root/.backup_password
    
    # IMPORTANTE: Salva password anche in chiaro per decriptazione
    echo "BACKUP_DECRYPT_PASSWORD=${backup_password}" > /root/.backup_decrypt_info
    chmod 600 /root/.backup_decrypt_info
    
    # Script backup
    cat > /usr/local/bin/nonuso-backup << 'BACKUP_EOF'
#!/bin/bash
set -euo pipefail

BACKUP_DIR="/opt/nonuso-backups"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_PATH="${BACKUP_DIR}/${DATE}"
LOG_FILE="/var/log/nonuso/backup.log"
GDRIVE_BASE="NonusoApp-VPS1"
BACKUP_PASSWORD_FILE="/root/.backup_password"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "${LOG_FILE}"
}

send_notification() {
    local subject="$1"
    local message="$2"
    echo "${message}" | mail -s "[NONUSO-SERVER] Backup: ${subject}" vmaltarello@gmail.com || true
}

mkdir -p "${BACKUP_PATH}"
log "=== INIZIO BACKUP ${DATE} ==="

# Backup Database
if docker ps --format '{{.Names}}' | grep -q "nonuso-postgres"; then
    log "Backup database..."
    docker exec nonuso-postgres pg_dumpall -U nonusouservm | gzip > "${BACKUP_PATH}/postgres_dump.sql.gz"
fi

# Backup volumi
log "Backup volumi Docker..."
for volume in $(docker volume ls -q | grep nonuso); do
    docker run --rm -v "${volume}:/source:ro" -v "${BACKUP_PATH}:/backup" alpine tar czf "/backup/${volume}.tar.gz" -C /source .
done

# Backup config
tar czf "${BACKUP_PATH}/configs.tar.gz" \
    /etc/nginx/sites-available \
    /etc/fail2ban/jail.local \
    /etc/docker/daemon.json \
    /home/unonuso/nonuso_net/docker-compose.prod.yml \
    2>/dev/null || true

# Backup secrets (criptato)
if [[ -d /home/unonuso/nonuso_net/secrets ]]; then
    log "Backup secrets (criptato)..."
    tar czf - /home/unonuso/nonuso_net/secrets | \
        openssl enc -aes-256-cbc -salt -pbkdf2 -iter 100000 \
        -pass file:"${BACKUP_PASSWORD_FILE}" \
        -out "${BACKUP_PATH}/secrets.tar.gz.enc"
fi

# IMPORTANTE: Includi info per decriptazione
cat > "${BACKUP_PATH}/DECRYPT_README.txt" << EOF
Per decriptare i file .enc:

openssl enc -aes-256-cbc -d -pbkdf2 -iter 100000 \
    -pass pass:VEDERE_PASSWORD_IN_GDRIVE \
    -in FILE.enc -out FILE

La password si trova in:
Google Drive/${GDRIVE_BASE}/credentials/backup-decrypt-password.txt
EOF

# Comprimi tutto
cd "${BACKUP_DIR}"
tar czf "${DATE}.tar.gz" "${DATE}"
rm -rf "${DATE}"

# Upload Google Drive
if command -v rclone &> /dev/null && rclone listremotes | grep -q "gdrive:"; then
    log "Upload su Google Drive..."
    
    # Crea struttura cartelle
    rclone mkdir "gdrive:/${GDRIVE_BASE}/backups/$(date +%Y-%m)" || true
    
    # Upload backup
    if rclone copy "${BACKUP_DIR}/${DATE}.tar.gz" "gdrive:/${GDRIVE_BASE}/backups/$(date +%Y-%m)/" --progress; then
        BACKUP_SIZE=$(du -h "${BACKUP_DIR}/${DATE}.tar.gz" | cut -f1)
        send_notification "Backup Completato" "Backup ${DATE} caricato su Google Drive (${BACKUP_SIZE})"
    else
        send_notification "Backup Warning" "Upload su Google Drive fallito per ${DATE}"
    fi
    
    # Pulizia remota backup più vecchi di 30 giorni
    rclone delete "gdrive:/${GDRIVE_BASE}/backups" --min-age 30d || true
fi

# Pulizia locale
find "${BACKUP_DIR}" -name "*.tar.gz" -type f -mtime +7 -delete

log "=== BACKUP COMPLETATO ==="
BACKUP_EOF

    chmod +x /usr/local/bin/nonuso-backup
    
    # Script per backup deploy da GitHub Actions
    cat > /usr/local/bin/backup-deploy << 'DEPLOY_EOF'
#!/bin/bash
set -euo pipefail

DEPLOY_BACKUP_DIR="/opt/deploy-backups"
DATE=$(date +%Y%m%d_%H%M%S)
APP_DIR="/home/unonuso/nonuso_net"
GDRIVE_BASE="NonusoApp-VPS1"

mkdir -p "${DEPLOY_BACKUP_DIR}"
chown unonuso:unonuso "${DEPLOY_BACKUP_DIR}"
chmod 750 "${DEPLOY_BACKUP_DIR}"

cd "${APP_DIR}"

# Backup pre-deploy
tar czf "${DEPLOY_BACKUP_DIR}/pre-deploy-${DATE}.tar.gz" \
    --exclude="node_modules" \
    --exclude=".git" \
    --exclude="*.log" \
    .

# Upload su Google Drive
if command -v rclone &> /dev/null && rclone listremotes | grep -q "gdrive:"; then
    rclone mkdir "gdrive:/${GDRIVE_BASE}/deploy-backups/$(date +%Y-%m)" || true
    rclone move "${DEPLOY_BACKUP_DIR}/pre-deploy-${DATE}.tar.gz" \
        "gdrive:/${GDRIVE_BASE}/deploy-backups/$(date +%Y-%m)/" || true
fi

# Pulizia locale
ls -t ${DEPLOY_BACKUP_DIR}/pre-deploy-*.tar.gz 2>/dev/null | tail -n +4 | xargs rm -f || true

# Pulizia Drive (più vecchi di 7 giorni)
rclone delete "gdrive:/${GDRIVE_BASE}/deploy-backups" --min-age 7d || true

echo "Deploy backup completato"
DEPLOY_EOF

    chmod +x /usr/local/bin/backup-deploy
    
    # Script restore da Google Drive
    cat > /usr/local/bin/nonuso-restore-gdrive << 'RESTORE_EOF'
#!/bin/bash
set -euo pipefail

GDRIVE_BASE="NonusoApp-VPS1"
LOCAL_RESTORE="/tmp/gdrive_restore"

if [[ $# -lt 1 ]]; then
    echo "Uso: $0 list|download|restore [backup_name]"
    exit 1
fi

case "$1" in
    list)
        echo "=== Backup disponibili su Google Drive ==="
        rclone ls "gdrive:/${GDRIVE_BASE}/backups" --max-depth 2 | grep ".tar.gz$"
        ;;
        
    download)
        if [[ -z "${2:-}" ]]; then
            echo "Specifica il nome del backup"
            exit 1
        fi
        mkdir -p "${LOCAL_RESTORE}"
        echo "Download ${2} da Google Drive..."
        rclone copy "gdrive:/${GDRIVE_BASE}/backups" "${LOCAL_RESTORE}" \
            --include "${2}" --progress
        echo "Download completato in: ${LOCAL_RESTORE}/${2}"
        ;;
        
    restore)
        if [[ -z "${2:-}" ]]; then
            echo "Specifica il nome del backup"
            exit 1
        fi
        $0 download "$2"
        echo "Avvio restore..."
        /usr/local/bin/nonuso-restore "${LOCAL_RESTORE}/${2}"
        rm -rf "${LOCAL_RESTORE}"
        ;;
        
    *)
        echo "Comando non valido: $1"
        exit 1
        ;;
esac
RESTORE_EOF

    chmod +x /usr/local/bin/nonuso-restore-gdrive
    
    # Cron backup
    cat > /etc/cron.d/nonuso-backup << 'CRON_EOF'
SHELL=/bin/bash
PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin

# Backup giornaliero alle 2 AM
0 2 * * * root /usr/local/bin/nonuso-backup >> /var/log/nonuso/backup-cron.log 2>&1

# Check spazio disco ogni 6 ore
0 */6 * * * root df -h | grep -E "^/dev/" | awk '{if(int($5)>85) print "Spazio disco warning: "$6" is "$5" full"}' | mail -s "[NONUSO-SERVER] Disk Space Alert" vmaltarello@gmail.com || true
CRON_EOF

    log SECURE "Backup configurato con Google Drive"
    
    # Upload immediato credenziali critiche
    if command -v rclone &> /dev/null && rclone listremotes | grep -q "gdrive:"; then
        log INFO "Upload credenziali critiche su Google Drive..."
        
        # Crea directory temporanea per credenziali
        local CRED_DIR="/tmp/credentials-$(date +%Y%m%d)"
        mkdir -p "${CRED_DIR}"
        
        # Copia tutti i file critici
        cp /root/.backup_password "${CRED_DIR}/backup-decrypt-password.txt"
        cp /root/.backup_decrypt_info "${CRED_DIR}/backup-decrypt-info.txt"
        cp /root/.ssh/github_actions_ed25519 "${CRED_DIR}/github_actions_ssh_key"
        cp /root/.ssh/github_actions_ed25519.pub "${CRED_DIR}/github_actions_ssh_key.pub"
        cp /root/nonuso-security-report.txt "${CRED_DIR}/" || true
        cp /root/.config/rclone/rclone.conf "${CRED_DIR}/rclone.conf" || true
        
        # Crea README per le credenziali
        cat > "${CRED_DIR}/README-IMPORTANTE.txt" << 'README_EOF'
CREDENZIALI CRITICHE NONUSO.NET
===============================

1. BACKUP DECRYPT PASSWORD
   File: backup-decrypt-password.txt
   Uso: Password per decriptare tutti i backup

2. GITHUB ACTIONS SSH KEY
   File: github_actions_ssh_key
   Uso: Chiave privata per deploy automatici

3. SECURITY REPORT
   File: nonuso-security-report.txt
   Contiene: Porte, configurazioni, comandi

4. RCLONE CONFIG
   File: rclone.conf
   Uso: Configurazione Google Drive

IMPORTANTE: Conserva questi file in modo sicuro!
Per decriptare backup: openssl enc -aes-256-cbc -d -pbkdf2 -iter 100000 -pass pass:PASSWORD -in file.enc -out file
README_EOF

        # Comprimi e cripta tutto
        cd /tmp
        tar czf - "credentials-$(date +%Y%m%d)" | \
            openssl enc -aes-256-cbc -salt -pbkdf2 -iter 100000 \
            -pass file:"/root/.backup_password" \
            -out "credentials-$(date +%Y%m%d).tar.gz.enc"
        
        # Upload su Drive
        rclone mkdir "gdrive:/${GDRIVE_BASE}/credentials" || true
        rclone copy "credentials-$(date +%Y%m%d).tar.gz.enc" "gdrive:/${GDRIVE_BASE}/credentials/" || true
        
        # Upload anche password in chiaro (per poter decriptare)
        echo "Password per decriptare: $(cat /root/.backup_password)" > "${CRED_DIR}/MASTER-DECRYPT-PASSWORD.txt"
        rclone copy "${CRED_DIR}/MASTER-DECRYPT-PASSWORD.txt" "gdrive:/${GDRIVE_BASE}/credentials/" || true
        
        # Cleanup
        rm -rf "${CRED_DIR}" "credentials-$(date +%Y%m%d).tar.gz.enc"
        
        log SECURE "Credenziali caricate su Drive"
    fi
}

# =============================================
# FASE 11: MONITORING
# =============================================

setup_monitoring() {
    log INFO "=== FASE 11: Monitoring Configuration ==="
    
    apt-get install -y monit
    
    # Crea directory per Monit
    mkdir -p /var/lib/monit
    chown -R root:root /var/lib/monit
    chmod 755 /var/lib/monit
    
    # Configurazione Monit
    log INFO "Configurando Monit..."
    cat > /etc/monit/monitrc << 'MONIT_EOF'
# Configurazione email
set mailserver smtp.gmail.com port 587
    username "vmaltarello@gmail.com" password "wqkq qqnv qqnv qqnv"
    using tlsv12
    with timeout 30 seconds

set mail-format {
    from: Monit <vmaltarello@gmail.com>
    subject: $SERVICE $EVENT at $DATE
    message: Monit $ACTION $SERVICE at $DATE on $HOST: $DESCRIPTION.
            Yours sincerely,
            monit
}

set alert vmaltarello@gmail.com

# Configurazione base
set daemon 60
set logfile /var/log/monit.log
set idfile /var/lib/monit/id
set statefile /var/lib/monit/state

# Interfaccia web
set httpd port 2812 and
    use address localhost
    allow localhost
    allow unonuso:monit

# Monitoraggio sistema
check system $HOSTNAME
    if loadavg (1min) > 4 then alert
    if loadavg (5min) > 2 then alert
    if memory usage > 80% then alert
    if swap usage > 20% then alert
    if cpu usage (user) > 80% then alert
    if cpu usage (system) > 20% then alert
    if cpu usage (wait) > 80% then alert

# Monitoraggio disco
check filesystem rootfs with path /
    if space usage > 80% then alert
    if inode usage > 80% then alert

# Monitoraggio Docker
check program docker with path "/usr/bin/docker info"
    if status != 0 then alert
    every 5 cycles

# Monitoraggio applicazione
check host api.nonuso.com with address api.nonuso.com
    if failed port 443 protocol https
        with timeout 10 seconds
        for 3 cycles
    then alert
    else if recovered then alert

# Monitoraggio servizi
check process docker with pidfile /var/run/docker.pid
    start program = "/usr/bin/systemctl start docker"
    stop program = "/usr/bin/systemctl stop docker"
    if failed unixsocket /var/run/docker.sock then restart
    if 5 restarts within 5 cycles then timeout

check process nginx with pidfile /var/run/nginx.pid
    start program = "/usr/bin/systemctl start nginx"
    stop program = "/usr/bin/systemctl stop nginx"
    if failed host api.nonuso.com port 443 protocol https then restart
    if 5 restarts within 5 cycles then timeout

check process certbot with pidfile /var/run/certbot.pid
    start program = "/usr/bin/systemctl start certbot"
    stop program = "/usr/bin/systemctl stop certbot"
    if 5 restarts within 5 cycles then timeout

check process rclone with pidfile /var/run/rclone.pid
    start program = "/usr/bin/systemctl start rclone"
    stop program = "/usr/bin/systemctl stop rclone"
    if 5 restarts within 5 cycles then timeout

check process monit with pidfile /var/run/monit.pid
    start program = "/usr/bin/systemctl start monit"
    stop program = "/usr/bin/systemctl stop monit"
    if 5 restarts within 5 cycles then timeout
MONIT_EOF

    chmod 600 /etc/monit/monitrc
    systemctl restart monit

    # Script monitor
    cat > /usr/local/bin/nonuso-monitor << 'MONITOR_EOF'
#!/bin/bash
echo "=== NONUSO.NET SYSTEM STATUS ==="
echo "Time: $(date)"
echo
echo "=== LOAD ==="
uptime
echo
echo "=== MEMORY ==="
free -h
echo
echo "=== DISK ==="
df -h
echo
echo "=== DOCKER ==="
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo
echo "=== FAIL2BAN ==="
fail2ban-client status | grep "Jail list" || echo "Fail2ban not running"
echo
echo "=== LAST LOGINS ==="
last -5
echo
echo "=== ACTIVE CONNECTIONS ==="
ss -tunap | grep ESTABLISHED | head -10
MONITOR_EOF

    chmod +x /usr/local/bin/nonuso-monitor
    
    log SECURE "Monitoring configurato"
}

# =============================================
# FASE 12: FINALIZZAZIONE
# =============================================

finalize_setup() {
    log INFO "=== FASE 12: Finalizzazione ==="
    
    # Disabilita servizi non necessari
    local services=("bluetooth" "cups" "avahi-daemon")
    for service in "${services[@]}"; do
        systemctl stop "${service}" 2>/dev/null || true
        systemctl disable "${service}" 2>/dev/null || true
    done
    
    # Rimuovi pacchetti non necessari
    apt-get purge -y telnet ftp rsh-client 2>/dev/null || true
    apt-get autoremove -y
    apt-get autoclean
    
    # Permessi file critici
    chmod 600 /etc/ssh/sshd_config
    chmod 600 /etc/crontab
    chmod 700 /root
    
    # Crea script gestione
    cat > /usr/local/bin/nonuso << 'NONUSO_EOF'
#!/bin/bash
case "$1" in
    start)
        cd /home/unonuso/nonuso_net
        docker compose -f docker-compose.prod.yml up -d
        ;;
    stop)
        cd /home/unonuso/nonuso_net
        docker compose -f docker-compose.prod.yml down
        ;;
    restart)
        $0 stop
        sleep 2
        $0 start
        ;;
    status)
        docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
        ;;
    logs)
        cd /home/unonuso/nonuso_net
        docker compose -f docker-compose.prod.yml logs -f ${2:-}
        ;;
    backup)
        /usr/local/bin/nonuso-backup
        ;;
    backup-deploy)
        /usr/local/bin/backup-deploy
        ;;
    monitor)
        /usr/local/bin/nonuso-monitor
        ;;
    *)
        echo "Usage: $0 {start|stop|restart|status|logs|backup|backup-deploy|monitor}"
        exit 1
        ;;
esac
NONUSO_EOF

    chmod +x /usr/local/bin/nonuso
    
    # Genera report finale
    cat > "${SECURITY_REPORT}" << REPORT_EOF
===============================================
   NONUSO.NET SECURITY SETUP REPORT
   Generated: $(date)
===============================================

1. ACCESSO SSH
--------------
Porta SSH: ${SSH_PORT}
Utente: ${APP_USER}
Auth: SOLO CHIAVE SSH

Connessione:
ssh -p ${SSH_PORT} ${APP_USER}@IP_SERVER

2. FIREWALL (UFW)
-----------------
Porte aperte:
- ${SSH_PORT}/tcp (SSH)
- 80/tcp (HTTP)
- 443/tcp (HTTPS)

3. SSL/HTTPS
------------
Dominio: ${DOMAIN}
Certificati: /etc/letsencrypt/live/${DOMAIN}/
Auto-renewal: Configurato

4. BACKUP
---------
Directory locale: ${BACKUP_DIR}
Google Drive: ${GDRIVE_BASE}/
Password decrypt: /root/.backup_password
Schedule: 2:00 AM giornaliero

5. EMAIL
--------
Notifiche a: ${EMAIL}
Tag filtro: [NONUSO-SERVER]

6. MONITORAGGIO
---------------
Monit: http://localhost:2812 (admin/monit)
SSH tunnel: ssh -L 2812:localhost:2812 user@server

7. COMANDI UTILI
----------------
nonuso start|stop|restart|status|logs|backup|monitor
nonuso-restore-gdrive list|download|restore

8. GITHUB ACTIONS
-----------------
Chiave salvata in: /root/.ssh/github_actions_ed25519
Backup deploy: /usr/local/bin/backup-deploy

9. GOOGLE DRIVE
---------------
Struttura:
${GDRIVE_BASE}/
├── backups/          # Backup database
├── credentials/      # Password e chiavi
└── deploy-backups/   # Backup pre-deploy

10. PROSSIMI PASSI
------------------
1. Testa connessione SSH sulla nuova porta:
   ssh -p ${SSH_PORT} ${APP_USER}@SERVER_IP

2. Aggiungi tua chiave SSH pubblica:
   echo "TUA_CHIAVE_PUBBLICA" >> /home/${APP_USER}/.ssh/authorized_keys

3. Disabilita password SSH:
   sed -i 's/PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config
   systemctl restart ssh

4. Configura DNS per ${DOMAIN}

5. Verifica backup su Drive:
   rclone ls gdrive:/${GDRIVE_BASE}/

===============================================
IMPORTANTE: Password decrypt backup salvata in:
- /root/.backup_password
- Google Drive/${GDRIVE_BASE}/credentials/
===============================================
REPORT_EOF

    chmod 600 "${SECURITY_REPORT}"
    
    log SECURE "Setup completato!"
}

# =============================================
# MAIN
# =============================================

main() {
    log INFO "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    log INFO "NONUSO.NET PRODUCTION SERVER SETUP v2.1"
    log INFO "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    # Trap errori
    trap 'log ERROR "Errore alla linea $LINENO. Exit code: $?"' ERR
    
    check_prerequisites
    setup_base_system
    harden_kernel
    setup_secure_user
    harden_ssh
    setup_firewall
    setup_fail2ban
    setup_docker
    setup_nginx
    setup_ssl
    setup_backup
    setup_monitoring
    finalize_setup
    
    echo
    log SECURE "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    log SECURE "SETUP COMPLETATO CON SUCCESSO!"
    log SECURE "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo
    log WARN "AZIONI IMMEDIATE RICHIESTE:"
    log WARN "1. SALVA la chiave GitHub Actions mostrata sopra"
    log WARN "2. NON disconnetterti ancora!"
    log WARN "3. Da ALTRA finestra terminal, testa SSH:"
    log WARN "   ssh -p ${SSH_PORT} ${APP_USER}@$(curl -s ifconfig.me)"
    log WARN "4. SOLO quando funziona, riavvia SSH:"
    log WARN "   systemctl restart ssh"
    echo
    log INFO "Password backup salvata in:"
    log INFO "- Locale: /root/.backup_password"
    log INFO "- Google Drive: ${GDRIVE_BASE}/credentials/MASTER-DECRYPT-PASSWORD.txt"
    echo
    cat "${SECURITY_REPORT}"
}

# Esegui
main "$@"