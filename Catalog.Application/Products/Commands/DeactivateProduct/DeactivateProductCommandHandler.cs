using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Products.Commands.DeactivateProduct
{
    public sealed class DeactivateProductCommandHandler
    {
        public Task Handle(
            DeactivateProductCommand command,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
