using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Products.Commands.ChangeProductPrice
{
    public sealed class ChangeProductPriceCommandHandler
    {
        public Task Handle(
            ChangeProductPriceCommand command,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
