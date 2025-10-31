using System;
using System.Collections.Generic;

namespace Catalog.Application.Products.Queries.GetProductById
{
    // DTO local a la query (no global)
    public sealed class GetProductByIdResult
    {
        public Guid Id { get; init; }
        public Guid VendorId { get; init; }
        public string Sku { get; init; } = "";
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public string Slug { get; init; } = "";
        public Guid CategoryId { get; init; }
        public decimal Price { get; init; }
        public int StockQuantity { get; init; }
        public bool IsActive { get; init; }
        public bool IsDiscontinued { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
        public string MainImageUrl { get; init; } = "";
        public IReadOnlyList<string> ImageUrls { get; init; } = Array.Empty<string>();
    }
}
