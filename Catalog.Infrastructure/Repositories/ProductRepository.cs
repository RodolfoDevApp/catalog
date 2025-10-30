using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Domain.Products;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _db;

        public ProductRepository(CatalogDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct)
        {
            return await _db.Products
                .FirstOrDefaultAsync(p => p.Id == productId, ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await _db.Products.AddAsync(product, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}
