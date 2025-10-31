using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Application.Common.Exceptions;
using Catalog.Application.Products.IntegrationEvents;
using MediatR;

namespace Catalog.Application.Products.Commands.AdjustProductStock
{
    public sealed class AdjustProductStockCommandHandler
        : IRequestHandler<AdjustProductStockCommand, Unit>
    {
        private readonly IProductRepository _repo;
        private readonly IOutboxWriter _outbox;

        public AdjustProductStockCommandHandler(IProductRepository repo, IOutboxWriter outbox)
        {
            _repo = repo;
            _outbox = outbox;
        }

        public async Task<Unit> Handle(AdjustProductStockCommand request, CancellationToken ct)
        {
            var product = await _repo.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                throw new NotFoundException("Product", request.ProductId);

            var now = DateTime.UtcNow;

            product.AdjustStock(request.Delta, now);

            await _outbox.AddMessageAsync(
                type: "ProductStockAdjusted",
                payload: new ProductStockAdjustedIntegrationEvent(
                    ProductId: product.Id,
                    Delta: request.Delta,
                    NewStock: product.StockQuantity,
                    Reason: "manual",
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
