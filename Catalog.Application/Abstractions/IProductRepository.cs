using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Domain.Products;

namespace Catalog.Application.Abstractions
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<(IReadOnlyList<Product> Items, long Total)> SearchAsync(
            ProductSearchCriteria criteria,
            CancellationToken ct);
    }
}
