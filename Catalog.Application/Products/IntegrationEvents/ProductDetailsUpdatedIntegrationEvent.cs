using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductDetailsUpdatedIntegrationEvent(
        Guid ProductId,
        string Name,
        string Description,
        string Slug,
        Guid CategoryId,
        DateTime UpdatedAtUtc
    );
}
