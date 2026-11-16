FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# adding curl and gpg for healthcheck
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl \
    gpg \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 5000
EXPOSE 5001
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG Configuration=debug

#restoring dependencies
COPY ./VictoryCenter/*.sln ./
COPY ./VictoryCenter/Directory.Build.props ./
COPY ./VictoryCenter/VictoryCenter.WebAPI/*.csproj ./VictoryCenter/VictoryCenter.WebAPI/packages.lock.json ./VictoryCenter.WebAPI/
COPY ./VictoryCenter/VictoryCenter.BLL/*.csproj ./VictoryCenter/VictoryCenter.BLL/packages.lock.json ./VictoryCenter.BLL/
COPY ./VictoryCenter/VictoryCenter.DAL/*.csproj ./VictoryCenter/VictoryCenter.DAL/packages.lock.json ./VictoryCenter.DAL/
COPY ./VictoryCenter/VictoryCenter.UnitTests/*.csproj ./VictoryCenter/VictoryCenter.UnitTests/packages.lock.json ./VictoryCenter.UnitTests/
COPY ./VictoryCenter/VictoryCenter.IntegrationTests/*.csproj ./VictoryCenter/VictoryCenter.IntegrationTests/packages.lock.json ./VictoryCenter.IntegrationTests/
COPY ./VictoryCenter/VictoryCenter.DbUpdate/*.csproj ./VictoryCenter/VictoryCenter.DbUpdate/packages.lock.json ./VictoryCenter.DbUpdate/
RUN dotnet restore --locked-mode

# copying other neccessary data and building application
COPY ./VictoryCenter/ ./
RUN dotnet build -c $Configuration -o /app/build --no-restore

# publishishing application
FROM build AS publish
RUN dotnet publish -c $Configuration -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./

LABEL atom="VictoryCenter"
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=15s --timeout=5s --start-period=60s --retries=5 \
    CMD http_status=$(curl --silent --show-error --output /dev/null \
        --connect-timeout 2 --max-time 4 --write-out '%{http_code}' \
        http://localhost:8080/health/ready) && [ "$http_status" = "200" ] || exit 1
ENTRYPOINT ["dotnet", "VictoryCenter.WebAPI.dll"]
