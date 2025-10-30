using System;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure.Persistence
{
    // Esta fábrica SOLO se usa en design-time (dotnet ef migrations ...).
    // No se ejecuta en producción ni cuando corre la API.
    //
    // Para no quemar credenciales fijas, primero intentamos leer una variable
    // de entorno (por ejemplo desde tu máquina o desde CI/CD); si no existe,
    // usamos una conexión local de dev.
    //
    // Ejemplo de uso en tu terminal antes de correr 'dotnet ef':
    //   $env:CATALOGDB_MIGRATIONS_CS="Host=localhost;Port=5432;Database=mydb;Username=admin;Password=admin"
    //
    public sealed class DesignTimeCatalogDbContextFactory
        : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var csFromEnv = Environment.GetEnvironmentVariable("CATALOGDB_MIGRATIONS_CS");

            var connectionString = string.IsNullOrWhiteSpace(csFromEnv)
                ? "Host=postgres;Port=5432;Database=mydb;Username=admin;Password=admin"
                : csFromEnv;

            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            // Útil durante desarrollo, no usar en prod
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();

            return new CatalogDbContext(optionsBuilder.Options);
        }
    }
}
