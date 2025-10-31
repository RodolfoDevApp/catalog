using System;
using MediatR;

namespace Catalog.Application.Products.Commands.AdjustProductStock
{
    // Sin "reason": el dominio solo acepta (delta, nowUtc)
    public sealed record AdjustProductStockCommand(
        Guid ProductId,
        int Delta
    ) : IRequest<Unit>;
}
