FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia csproj y nuget.config para cache del restore
COPY nuget.config ./
COPY Catalog.Api/Catalog.Api.csproj Catalog.Api/
COPY Catalog.Application/Catalog.Application.csproj Catalog.Application/
COPY Catalog.Domain/Catalog.Domain.csproj Catalog.Domain/
COPY Catalog.Infrastructure/Catalog.Infrastructure.csproj Catalog.Infrastructure/

# Usa el nuget.config con credenciales
RUN dotnet restore Catalog.Api/Catalog.Api.csproj --configfile nuget.config

# Copia todo y publica
COPY . .
WORKDIR /src/Catalog.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Catalog.Api.dll"]
