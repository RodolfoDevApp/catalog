using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Application.Common.Exceptions;
using Catalog.Application.Products.IntegrationEvents;
using MediatR;

namespace Catalog.Application.Products.Commands.UpdateProductDetails
{
    public sealed class UpdateProductDetailsCommandHandler
        : IRequestHandler<UpdateProductDetailsCommand, Unit>
    {
        private readonly IProductRepository _repo;
        private readonly IOutboxWriter _outbox;

        public UpdateProductDetailsCommandHandler(IProductRepository repo, IOutboxWriter outbox)
        {
            _repo = repo;
            _outbox = outbox;
        }

        public async Task<Unit> Handle(UpdateProductDetailsCommand request, CancellationToken ct)
        {
            var product = await _repo.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                throw new NotFoundException("Product", request.ProductId);

            var now = DateTime.UtcNow;

            // El dominio exige mainImageUrl e imageUrls; si no vienen en el comando,
            // se conservan los valores actuales del agregado.
            product.UpdateDetails(
                name: request.Name,
                description: request.Description,
                slug: request.Slug,
                categoryId: request.CategoryId,
                mainImageUrl: product.MainImageUrl,
                imageUrls: product.ImageUrls,
                nowUtc: now
            );

            await _outbox.AddMessageAsync(
                type: "ProductDetailsUpdated",
                payload: new ProductDetailsUpdatedIntegrationEvent(
                    ProductId: product.Id,
                    Name: product.Name,
                    Slug: product.Slug,
                    CategoryId: product.CategoryId,
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
