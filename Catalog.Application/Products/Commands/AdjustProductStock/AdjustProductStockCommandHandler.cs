using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Products.Commands.AdjustProductStock
{
    public sealed class AdjustProductStockCommandHandler
    {
        public Task Handle(
            AdjustProductStockCommand command,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
