# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080


RUN apt-get update \
    && apt-get install -y ghostscript fluidsynth wget unzip default-jre \
    # אפשר לנסות גם musescore, אם קיים:
    && apt-get install -y musescore || echo "musescore not available, skipping" \
    # התקנת Audiveris
    && wget https://github.com/Audiveris/audiveris/releases/download/5.6.1/Audiveris-5.6.1-ubuntu22.04-x86_64.deb \
    && dpkg -i Audiveris-5.6.1-ubuntu22.04-x86_64.deb || apt-get install -f -y \
    && rm -rf Audiveris-5.6.1-ubuntu22.04-x86_64.deb \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && which gs \
    && gs --version

USER $APP_UID
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Music.Api/Music.Api.csproj", "Music.Api/"]
COPY ["Music.Core/Music.Core.csproj", "Music.Core/"]
COPY ["Music.Data/Music.Data.csproj", "Music.Data/"]
COPY ["Music.Service/Music.Service.csproj", "Music.Service/"]
RUN dotnet restore "./Music.Api/Music.Api.csproj"
COPY . .
WORKDIR "/src/Music.Api"
RUN dotnet build "./Music.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Music.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Music.Api.dll"]