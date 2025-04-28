FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env

WORKDIR /src

COPY "GameClient/*.csproj" "GameClient/"
COPY "GameShared/*.csproj" "GameShared/"

RUN dotnet restore "./GameClient/GameClient.csproj"

COPY "GameClient/" "GameClient/"
COPY "GameShared/" "GameShared/"

WORKDIR /src/GameClient

RUN dotnet publish -o /app/output

FROM nginx:alpine AS runtime

WORKDIR /var/www/web

COPY --from=build-env /app/output/wwwroot .

COPY GameClient/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

