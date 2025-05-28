#!/bin/bash
set -e

# Aggiungi la cartella dotnet tools alla PATH
export PATH="$PATH:/root/.dotnet/tools"

# Installa dotnet-ef se non presente
if ! command -v dotnet-ef &> /dev/null; then
    dotnet tool install --global dotnet-ef --version 9.0.5
fi


dotnet ef database update --project Infrastructure/Infrastructure.Persistence/Nonuso.Infrastructure.Persistence.csproj --startup-project Api/Nonuso.Api.csproj --verbose