using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductStockAdjustedIntegrationEvent(
        Guid ProductId,
        int NewStockQuantity,
        DateTime AdjustedAtUtc
    );
}
