using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductStockAdjustedIntegrationEvent(
        Guid ProductId,
        int Delta,
        int NewStock,
        string Reason,
        DateTime UpdatedAtUtc
    );
}
