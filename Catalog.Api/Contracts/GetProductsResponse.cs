using System;
using System.Collections.Generic;

namespace Catalog.Api.Contracts
{
    // Wrapper 2xx para el listado
    public sealed class GetProductsResponse
    {
        public bool Success { get; init; } = true;
        public int Status { get; init; } = 200;
        public string Message { get; init; } = "OK";
        public GetProductsData Data { get; init; } = new();
        public object? Meta { get; init; }
        public string? TraceId { get; init; }
        public string Service { get; init; } = "catalog-api";
        public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

        public sealed class GetProductsData
        {
            public IReadOnlyList<Item> Items { get; init; } = Array.Empty<Item>();
        }

        public sealed class Item
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
}
