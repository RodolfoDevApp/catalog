using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Application.Common.Exceptions;
using Catalog.Application.Products.IntegrationEvents;
using MediatR;

namespace Catalog.Application.Products.Commands.DiscontinueProduct
{
    public sealed class DiscontinueProductCommandHandler
        : IRequestHandler<DiscontinueProductCommand, Unit>
    {
        private readonly IProductRepository _repo;
        private readonly IOutboxWriter _outbox;

        public DiscontinueProductCommandHandler(IProductRepository repo, IOutboxWriter outbox)
        {
            _repo = repo;
            _outbox = outbox;
        }

        public async Task<Unit> Handle(DiscontinueProductCommand request, CancellationToken ct)
        {
            var product = await _repo.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                throw new NotFoundException("Product", request.ProductId);

            var now = DateTime.UtcNow;

            product.Discontinue(now);

            await _outbox.AddMessageAsync(
                type: "ProductDiscontinued",
                payload: new ProductDiscontinuedIntegrationEvent(
                    ProductId: product.Id,
                    UpdatedAtUtc: now
                ),
                occurredAtUtc: now,
                ct: ct
            );

            await _repo.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
