#!/bin/bash
set -e

# Aggiungi la cartella dotnet tools alla PATH
export PATH="$PATH:/root/.dotnet/tools"

echo "Installing dotnet-ef tool..."
dotnet tool install --global dotnet-ef

echo "Running database migrations..."
echo "Current directory: $(pwd)"
echo "Listing files in current directory:"
ls -la

echo "Building in Release mode..."
dotnet build -c Release

echo "Running migrations..."
dotnet ef database update --project Infrastructure/Infrastructure.Persistence/Nonuso.Infrastructure.Persistence.csproj --startup-project Api/Nonuso.Api.csproj --configuration Release --verbose