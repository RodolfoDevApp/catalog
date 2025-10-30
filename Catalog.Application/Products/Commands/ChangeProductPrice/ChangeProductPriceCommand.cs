using System;

namespace Catalog.Application.Products.Commands.ChangeProductPrice
{
    public sealed record ChangeProductPriceCommand(
        Guid ProductId,
        decimal NewPrice
    );
}
