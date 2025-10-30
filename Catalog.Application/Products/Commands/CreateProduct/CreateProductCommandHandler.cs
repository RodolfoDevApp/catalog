using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Application.Common.Exceptions;
using Catalog.Application.Products.IntegrationEvents;
using Catalog.Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Catalog.Application.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandHandler
        : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IOutboxWriter _outboxWriter;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IOutboxWriter outboxWriter)
        {
            _productRepository = productRepository;
            _outboxWriter = outboxWriter;
        }

        public async Task<Guid> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var nowUtc = DateTime.UtcNow;

            // 1. construir agregado Product con toda la info,
            //    incluyendo imágenes
            var product = Product.Create(
                vendorId: request.VendorId,
                sku: request.Sku,
                name: request.Name,
                description: request.Description,
                slug: request.Slug,
                categoryId: request.CategoryId,
                price: request.Price,
                initialStock: request.InitialStock,
                isActive: request.IsActive,
                nowUtc: nowUtc,
                mainImageUrl: request.MainImageUrl,
                imageUrls: request.ImageUrls
            );

            try
            {
                // 2. stage en DbContext (todavía sin commit)
                await _productRepository.AddAsync(product, cancellationToken);

                // 3. armar integration event para otros servicios
                var integrationEvent = new ProductCreatedIntegrationEvent(
                    ProductId: product.Id,
                    VendorId: product.VendorId,
                    Sku: product.Sku,
                    Name: product.Name,
                    Slug: product.Slug,
                    Price: product.Price,
                    StockQuantity: product.StockQuantity,
                    CreatedAtUtc: product.CreatedAtUtc,
                    MainImageUrl: product.MainImageUrl,
                    ImageUrls: product.ImageUrls
                );

                // 4. dejar mensaje en outbox (también tracked por el mismo DbContext)
                await _outboxWriter.AddMessageAsync(
                    type: "ProductCreated",
                    payload: integrationEvent,
                    occurredAtUtc: nowUtc,
                    ct: cancellationToken
                );

                // 5. commit transaccional
                await _productRepository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException dbEx) when (IsUniqueSkuViolation(dbEx))
            {
                // sku duplicado -> 409 Conflict
                throw new ConflictException($"SKU '{request.Sku}' ya existe.");
            }

            // 6. regreso el id
            return product.Id;
        }

        private static bool IsUniqueSkuViolation(DbUpdateException ex)
        {
            // Postgres: SqlState "23505" = unique_violation
            // y usamos constraint ux_products_sku en ProductConfiguration
            if (ex.InnerException is PostgresException pg &&
                pg.SqlState == "23505" &&
                pg.ConstraintName == "ux_products_sku")
            {
                return true;
            }

            return false;
        }
    }
}
