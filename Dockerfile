# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solution e csproj primeiro (cache do restore)
COPY ./apitesteserverlinux.sln ./
COPY ./apitesteserverlinux.Api/apitesteserverlinux.Api.csproj ./apitesteserverlinux.Api/
COPY ./apitesteserverlinux.Domain/apitesteserverlinux.Domain.csproj ./apitesteserverlinux.Domain/

# Restore usando a solution (pega dependências entre projetos)
RUN dotnet restore ./apitesteserverlinux.sln

# Copia o restante do repositório
COPY . .

# Publish da API
RUN dotnet publish ./apitesteserverlinux.Api/apitesteserverlinux.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "apitesteserverlinux.Api.dll"]