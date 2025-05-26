# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files
COPY ["Api/Nonuso.Api.csproj", "Api/"]
COPY ["Application/Nonuso.Application.csproj", "Application/"]
COPY ["Domain/Nonuso.Domain.csproj", "Domain/"]
COPY ["Common/Nonuso.Common.csproj", "Common/"]
COPY ["Messages/Nonuso.Messages.csproj", "Messages/"]
COPY ["Infrastructure/Infrastructure.Auth/Nonuso.Infrastructure.Auth.csproj", "Infrastructure/Infrastructure.Auth/"]
COPY ["Infrastructure/Infrastructure.Persistence/Nonuso.Infrastructure.Persistence.csproj", "Infrastructure/Infrastructure.Persistence/"]
COPY ["Infrastructure/Infrastructure.Storage/Nonuso.Infrastructure.Storage.csproj", "Infrastructure/Infrastructure.Storage/"]
COPY ["Infrastructure/Infrastructure.Notification/Nonuso.Infrastructure.Notification.csproj", "Infrastructure/Infrastructure.Notification/"]
COPY ["Infrastructure/Infrastructure.Redis/Nonuso.Infrastructure.Redis.csproj", "Infrastructure/Infrastructure.Redis/"]

# Restore and build
RUN dotnet restore "Api/Nonuso.Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Nonuso.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nonuso.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image (runtime)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# ✅ Make sure dotnet-ef is available in PATH
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install -g dotnet-ef --version 9.0.0

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Nonuso.Api.dll"]