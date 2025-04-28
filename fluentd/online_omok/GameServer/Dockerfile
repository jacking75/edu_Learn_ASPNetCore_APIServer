FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env
WORKDIR /src

COPY "GameServer/*.csproj" "GameServer/"
COPY "GameShared/*.csproj" "GameShared/"
COPY "ServerShared/*.csproj" "ServerShared/"

RUN dotnet restore "./GameServer/GameServer.csproj"

COPY "GameServer/" "GameServer/"
COPY "GameShared/" "GameShared/"
COPY "ServerShared/" "ServerShared/"
WORKDIR "/src/GameServer"

RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite

WORKDIR /app
EXPOSE 8000
COPY --from=build-env /app .
ENTRYPOINT ["./GameServer"]