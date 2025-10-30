using System;
using System.Collections.Generic;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductCreatedIntegrationEvent(
        Guid ProductId,
        Guid VendorId,
        string Sku,
        string Name,
        string Slug,
        decimal Price,
        int StockQuantity,
        DateTime CreatedAtUtc,
        string? MainImageUrl,
        IReadOnlyList<string> ImageUrls
    );
}
