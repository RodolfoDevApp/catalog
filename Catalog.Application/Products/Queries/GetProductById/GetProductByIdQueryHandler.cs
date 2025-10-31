using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Application.Common.Exceptions;
using MediatR;

namespace Catalog.Application.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        private readonly IProductRepository _repo;

        public GetProductByIdQueryHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(request.ProductId, ct);
            if (p is null)
                throw new NotFoundException("Product", request.ProductId.ToString());

            return new GetProductByIdResult
            {
                Id = p.Id,
                VendorId = p.VendorId,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Slug = p.Slug,
                CategoryId = p.CategoryId,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                IsDiscontinued = p.IsDiscontinued,
                CreatedAtUtc = p.CreatedAtUtc,
                UpdatedAtUtc = p.UpdatedAtUtc,
                MainImageUrl = p.MainImageUrl,
                ImageUrls = p.ImageUrls
            };
        }
    }
}
