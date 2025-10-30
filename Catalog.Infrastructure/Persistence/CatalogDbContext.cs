using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence
{
    // DbContext del bounded context "Catalog".
    //
    // Importante:
    // - Este contexto contiene tanto entidades de dominio (Product)
    //   como entidades técnicas (OutboxMessage).
    //
    // - SaveChangesAsync() se usará dentro de los handlers para guardar
    //   el agregado Product y su OutboxMessage en la MISMA transacción.
    //
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica las configuraciones Fluent de cada entidad
            modelBuilder.ApplyConfiguration(
                new Configurations.ProductConfiguration());

            modelBuilder.ApplyConfiguration(
                new Configurations.OutboxMessageConfiguration());
        }
    }
}
