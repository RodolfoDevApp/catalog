using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Domain.Products;

namespace Catalog.Application.Abstractions
{
    // Puerto de acceso al agregado Product
    // Implementado en Infrastructure.Repositories.ProductRepository
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
