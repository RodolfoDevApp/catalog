using System;
using System.Collections.Generic;
using System.Linq;
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

        public ProductRepository(CatalogDbContext db) => _db = db;

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

        // NUEVO: consulta paginada/filtrada manteniendo IQueryable hasta el final
        public async Task<(IReadOnlyList<Product> Items, long Total)> SearchAsync(
            ProductSearchCriteria c, CancellationToken ct)
        {
            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(c.Search))
            {
                var s = $"%{c.Search.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.ILike(p.Sku, s) ||
                    EF.Functions.ILike(p.Name, s) ||
                    EF.Functions.ILike(p.Description, s) ||
                    EF.Functions.ILike(p.Slug, s));
            }

            if (c.VendorId is Guid vendor) query = query.Where(p => p.VendorId == vendor);
            if (c.CategoryId is Guid cat) query = query.Where(p => p.CategoryId == cat);
            if (c.MinPrice is decimal min) query = query.Where(p => p.Price >= min);
            if (c.MaxPrice is decimal max) query = query.Where(p => p.Price <= max);
            if (c.Active.HasValue) query = query.Where(p => p.IsActive == c.Active.Value);
            if (c.Discontinued.HasValue) query = query.Where(p => p.IsDiscontinued == c.Discontinued.Value);

            var sortBy = (c.SortBy ?? "createdAt").ToLowerInvariant();
            var desc = (c.SortDir ?? "desc").ToLowerInvariant() != "asc";

            query = sortBy switch
            {
                "name" => desc ? query.OrderByDescending(p => p.Name).ThenByDescending(p => p.Id) : query.OrderBy(p => p.Name).ThenBy(p => p.Id),
                "price" => desc ? query.OrderByDescending(p => p.Price).ThenByDescending(p => p.Id) : query.OrderBy(p => p.Price).ThenBy(p => p.Id),
                "sku" => desc ? query.OrderByDescending(p => p.Sku).ThenByDescending(p => p.Id) : query.OrderBy(p => p.Sku).ThenBy(p => p.Id),
                _ => desc ? query.OrderByDescending(p => p.CreatedAtUtc).ThenByDescending(p => p.Id) : query.OrderBy(p => p.CreatedAtUtc).ThenBy(p => p.Id),
            };

            var page = c.Page < 1 ? 1 : c.Page;
            var size = c.PageSize <= 0 ? 20 : Math.Min(c.PageSize, 100);

            var total = await query.LongCountAsync(ct);

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
