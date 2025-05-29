# Nonuso.net Server Setup Guide

Questa guida descrive il processo di configurazione di un nuovo server VPS per Nonuso.net.

## Prerequisiti

- Un server VPS con Ubuntu 22.04 LTS
- Accesso root o utente con privilegi sudo
- Un dominio configurato (api.nonuso.com)
- Accesso SSH al server

## Setup Iniziale

1. Connettiti al server via SSH:
```bash
ssh root@your-server-ip
```

2. Crea un nuovo utente non-root:
```bash
adduser nonuso
usermod -aG sudo nonuso
```

3. Configura SSH per il nuovo utente:
```bash
mkdir -p /home/nonuso/.ssh
cp ~/.ssh/authorized_keys /home/nonuso/.ssh/
chown -R nonuso:nonuso /home/nonuso/.ssh
chmod 700 /home/nonuso/.ssh
chmod 600 /home/nonuso/.ssh/authorized_keys
```

4. Disabilita l'accesso root via SSH:
```bash
sudo nano /etc/ssh/sshd_config
```
Modifica o aggiungi:
```
PermitRootLogin no
PasswordAuthentication no
```

5. Riavvia SSH:
```bash
sudo systemctl restart sshd
```

## Esecuzione dello Script di Setup

1. Copia lo script di setup sul server:
```bash
scp server-setup.sh nonuso@your-server-ip:/home/nonuso/
```

2. Connettiti come utente non-root:
```bash
ssh nonuso@your-server-ip
```

3. Rendi lo script eseguibile:
```bash
chmod +x server-setup.sh
```

4. Esegui lo script:
```bash
./server-setup.sh
```

## Configurazione Post-Setup

### 1. DNS Configuration
Configura i record DNS per api.nonuso.com:
- A Record: punta al tuo IP del server
- CNAME Record: www.api.nonuso.com -> api.nonuso.com

### 2. SSL Certificate
Lo script configura automaticamente Let's Encrypt, ma verifica:
```bash
sudo certbot certificates
```

### 3. Monitoring Setup

#### Grafana
1. Accedi a Grafana (http://your-server-ip:3000)
2. Credenziali default:
   - Username: admin
   - Password: your_secure_password
3. Configura le dashboard:
   - Importa dashboard ID: 1860 (Node Exporter)
   - Importa dashboard ID: 893 (Docker)

#### Prometheus
1. Verifica l'accesso: http://your-server-ip:9090
2. Configura le regole di alerting in `/etc/prometheus/rules/`

### 4. Backup Verification
1. Esegui manualmente il backup:
```bash
sudo /opt/nonuso-net/backup.sh
```

2. Verifica i backup:
```bash
ls -l /opt/nonuso-backups/
```

### 5. Security Checks
1. Verifica Fail2Ban:
```bash
sudo fail2ban-client status
```

2. Verifica UFW:
```bash
sudo ufw status verbose
```

3. Verifica i log:
```bash
sudo tail -f /var/log/nginx/error.log
sudo tail -f /var/log/fail2ban.log
```

## Manutenzione

### Backup
I backup vengono eseguiti automaticamente ogni giorno alle 2:00 AM.
Per eseguire un backup manuale:
```bash
sudo /opt/nonuso-net/backup.sh
```

### Log Rotation
I log vengono ruotati automaticamente:
- Nginx logs: ogni giorno
- ModSecurity logs: ogni giorno
- Mantenuti per 14 giorni

### Updates
Gli aggiornamenti di sicurezza vengono installati automaticamente.
Per aggiornamenti manuali:
```bash
sudo apt update && sudo apt upgrade
```

## Troubleshooting

### Nginx Issues
1. Verifica la configurazione:
```bash
sudo nginx -t
```

2. Controlla i log:
```bash
sudo tail -f /var/log/nginx/error.log
```

### Database Issues
1. Verifica lo stato:
```bash
docker ps | grep postgres
```

2. Controlla i log:
```bash
docker logs nonuso-postgres
```

### Redis Issues
1. Verifica lo stato:
```bash
docker ps | grep redis
```

2. Controlla i log:
```bash
docker logs nonuso-redis
```

## Monitoring e Alerting

### Grafana Alerts
Configura gli alert in Grafana per:
- CPU usage > 80%
- Memory usage > 90%
- Disk usage > 85%
- API response time > 1s
- Error rate > 5%

### Prometheus Rules
Le regole di alerting sono configurate in `/etc/prometheus/rules/`:
- High CPU usage
- High memory usage
- High disk usage
- Service down
- High error rate

## Security Best Practices

1. Mantieni il sistema aggiornato
2. Monitora i log regolarmente
3. Verifica i backup settimanalmente
4. Controlla le regole del firewall
5. Verifica i certificati SSL
6. Monitora le performance
7. Controlla gli accessi non autorizzati

## Support

Per problemi o domande:
1. Controlla i log
2. Verifica lo stato dei servizi
3. Controlla la documentazione
4. Contatta il supporto tecnico 