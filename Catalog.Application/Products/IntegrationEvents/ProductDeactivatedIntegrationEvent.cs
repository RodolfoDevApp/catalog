using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductDeactivatedIntegrationEvent(
        Guid ProductId,
        DateTime UpdatedAtUtc
    );
}
