using System;
using MediatR;

namespace Catalog.Application.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(
        int Page,
        int PageSize,
        string? Search,
        Guid? VendorId,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        bool? Active,
        bool? Discontinued,
        string? SortBy,
        string? SortDir
    ) : IRequest<GetProductsPage>;
}
