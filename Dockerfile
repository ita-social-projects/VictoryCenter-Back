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
COPY ./VictoryCenter/VictoryCenter.WebAPI/*.csproj ./VictoryCenter.WebAPI/
COPY ./VictoryCenter/VictoryCenter.BLL/*.csproj ./VictoryCenter.BLL/
COPY ./VictoryCenter/VictoryCenter.DAL/*.csproj ./VictoryCenter.DAL/
COPY ./VictoryCenter/VictoryCenter.UnitTests/*.csproj ./VictoryCenter.UnitTests/
COPY ./VictoryCenter/VictoryCenter.IntegrationTests/*.csproj ./VictoryCenter.IntegrationTests/
RUN dotnet restore

# copying other neccessary data and building application
COPY ./VictoryCenter/ ./
RUN dotnet build -c $Configuration -o /app/build

# publishishing application
FROM build AS publish
RUN dotnet publish -c $Configuration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./

LABEL atom="VictoryCenter"
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "VictoryCenter.WebAPI.dll"]
