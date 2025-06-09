Write-Host "Avvio ambiente di sviluppo Nonuso API..." -ForegroundColor Green

if (!(Test-Path ".\secrets")) {
    Write-Host "Cartella secrets non trovata!" -ForegroundColor Red
    Write-Host "   Assicurati che la cartella .\secrets\ esista nella root del progetto" -ForegroundColor Yellow
    exit 1
}

if (!(Test-Path ".\docker-compose.debug.yml")) {
    Write-Host "File docker-compose.debug.yml non trovato!" -ForegroundColor Red
    Write-Host "   Crea il file docker-compose.debug.yml nella root del progetto" -ForegroundColor Yellow
    exit 1
}

Write-Host "Fermando container esistenti..." -ForegroundColor Yellow
docker-compose -f docker-compose.debug.yml down

Write-Host "Pulizia immagini non utilizzate..." -ForegroundColor Yellow
docker system prune -f


Write-Host "Avvio servizi..." -ForegroundColor Green
docker-compose -f docker-compose.debug.yml up -d --build

Write-Host "Attendendo che i servizi siano pronti..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "Stato dei container:" -ForegroundColor Cyan
docker-compose -f docker-compose.debug.yml ps

Write-Host ""
Write-Host "Log recenti API:" -ForegroundColor Cyan
docker logs nonuso-api --tail 15

Write-Host ""
Write-Host "Ambiente di development pronto!" -ForegroundColor Green
Write-Host "Servizi disponibili:" -ForegroundColor Yellow
Write-Host "   API: http://localhost:8080" -ForegroundColor White
Write-Host "   API (HTTPS): https://localhost:8081" -ForegroundColor White
Write-Host "   PostgreSQL: localhost:5432" -ForegroundColor White
Write-Host "   Redis: localhost:6379" -ForegroundColor White

Write-Host ""
Write-Host "Per il debugging con Visual Studio:" -ForegroundColor Cyan
Write-Host "   Debug: Attach to Process..." -ForegroundColor White
Write-Host "   Connection Type: Docker" -ForegroundColor White
Write-Host "   Container: nonuso-api" -ForegroundColor White
Write-Host "   Process: dotnet" -ForegroundColor White

Write-Host ""
Write-Host "Comandi utili:" -ForegroundColor Magenta
Write-Host "   Logs in tempo reale: docker logs nonuso-api -f" -ForegroundColor White
Write-Host "   Accedi al container: docker exec -it nonuso-api bash" -ForegroundColor White
Write-Host "   Ferma tutto: docker-compose -f docker-compose.debug.yml down" -ForegroundColor White

try {
    $logs = docker logs nonuso-api --tail 5 2>&1
    if ($logs -match "error|exception|fail") {
        Write-Host ""
        Write-Host "Rilevati possibili errori nei log. Controlla i dettagli sopra." -ForegroundColor Red
    } else {
        Write-Host ""
        Write-Host "Tutto sembra funzionare correttamente!" -ForegroundColor Green
    }
} catch {
    Write-Host ""
    Write-Host "Impossibile verificare i log del container." -ForegroundColor Yellow
}