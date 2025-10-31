using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using MediatR;

namespace Catalog.Application.Products.Queries.GetProducts
{
    public sealed class GetProductsQueryHandler
        : IRequestHandler<GetProductsQuery, GetProductsPage>
    {
        private readonly IProductRepository _repo;

        public GetProductsQueryHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetProductsPage> Handle(GetProductsQuery request, CancellationToken ct)
        {
            var criteria = new ProductSearchCriteria
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                VendorId = request.VendorId,
                CategoryId = request.CategoryId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Active = request.Active,
                Discontinued = request.Discontinued,
                SortBy = request.SortBy ?? "createdAt",
                SortDir = request.SortDir ?? "desc"
            };

            var (items, total) = await _repo.SearchAsync(criteria, ct);

            var page = criteria.Page < 1 ? 1 : criteria.Page;
            var size = criteria.PageSize <= 0 ? 20 : (criteria.PageSize > 100 ? 100 : criteria.PageSize);
            var totalPages = (int)((total + size - 1) / size);

            return new GetProductsPage
            {
                Items = items.Select(p => new GetProductsPage.Item
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
                }).ToList(),
                Page = page,
                PageSize = size,
                TotalItems = total,
                TotalPages = totalPages
            };
        }
    }
}
