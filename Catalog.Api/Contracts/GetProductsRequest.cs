using System;

namespace Catalog.Api.Contracts
{
    // Querystring para GET /products
    public sealed class GetProductsRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public string? Search { get; init; }
        public Guid? VendorId { get; init; }
        public Guid? CategoryId { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public bool? Active { get; init; }
        public bool? Discontinued { get; init; }

        // createdAt | name | price | sku
        public string? SortBy { get; init; } = "createdAt";
        // asc | desc
        public string? SortDir { get; init; } = "desc";
    }
}
