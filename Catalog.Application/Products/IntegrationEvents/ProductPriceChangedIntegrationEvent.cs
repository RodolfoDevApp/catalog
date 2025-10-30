using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductPriceChangedIntegrationEvent(
        Guid ProductId,
        Guid VendorId,
        decimal NewPrice,
        DateTime ChangedAtUtc
    );
}
