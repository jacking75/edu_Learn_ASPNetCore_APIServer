# Build App

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /App

COPY . ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out


# Build Runtime Image

EXPOSE 8000
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "TestApiServer.dll"]