using System;

namespace Catalog.Application.Products.Commands.AdjustProductStock
{
    public sealed record AdjustProductStockCommand(
        Guid ProductId,
        int DeltaQuantity
    );
}
