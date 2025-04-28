# Use the official ASP.NET runtime image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5105
EXPOSE 7080

# Use the official build image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and restore dependencies
COPY ["GameServer.csproj", "./"]

RUN dotnet restore "./GameServer.csproj"

# Copy the rest of the project files
COPY . .

# Build the project
WORKDIR "/src/."
RUN dotnet build "GameServer.csproj" -c Release -o /app/build

# Publish the build
FROM build AS publish
RUN dotnet publish "GameServer.csproj" -c Release -o /app/publish

# Final stage - run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GameServer.dll"]