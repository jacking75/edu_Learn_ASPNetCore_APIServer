FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env
WORKDIR /src

COPY "MatchAPIServer/*.csproj" "MatchAPIServer/"
COPY "GameShared/*.csproj" "GameShared/"
COPY "ServerShared/*.csproj" "ServerShared/"

RUN dotnet restore "./MatchAPIServer/MatchAPIServer.csproj"

COPY "MatchAPIServer/" "MatchAPIServer/"
COPY "GameShared/" "GameShared/"
COPY "ServerShared/" "ServerShared/"
WORKDIR "/src/MatchAPIServer"

RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite

WORKDIR /app
EXPOSE 9000
COPY --from=build-env /app .
ENTRYPOINT ["./MatchAPIServer"]