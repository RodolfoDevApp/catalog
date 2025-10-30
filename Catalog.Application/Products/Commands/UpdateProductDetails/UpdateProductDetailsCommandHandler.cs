using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Products.Commands.UpdateProductDetails
{
    public sealed class UpdateProductDetailsCommandHandler
    {
        public Task Handle(
            UpdateProductDetailsCommand command,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
