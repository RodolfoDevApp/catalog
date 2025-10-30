using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler
    {
        public Task<ProductDto?> Handle(
            GetProductByIdQuery query,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    public sealed record ProductDto(
        Guid Id,
        Guid VendorId,
        string Sku,
        string Name,
        string Description,
        string Slug,
        Guid CategoryId,
        decimal Price,
        int StockQuantity,
        bool IsActive,
        bool IsDiscontinued,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc
    );
}
