FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env
WORKDIR /src

COPY "HiveAPIServer/*.csproj" "HiveAPIServer/"
COPY "GameShared/*.csproj" "GameShared/"
COPY "ServerShared/*.csproj" "ServerShared/"

RUN dotnet restore "./HiveAPIServer/HiveAPIServer.csproj"

COPY "HiveAPIServer/" "HiveAPIServer/"
COPY "GameShared/" "GameShared/"
COPY "ServerShared/" "ServerShared/"
WORKDIR "/src/HiveAPIServer"

RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite

WORKDIR /app
EXPOSE 8080
COPY --from=build-env /app .
ENTRYPOINT ["./HiveAPIServer"]