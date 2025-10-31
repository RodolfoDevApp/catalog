using System;

namespace Catalog.Application.Products.IntegrationEvents
{
    public sealed record ProductDiscontinuedIntegrationEvent(
        Guid ProductId,
        DateTime UpdatedAtUtc
    );
}
