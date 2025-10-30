using MediatR;
using System;
using System.Collections.Generic;

namespace Catalog.Application.Products.Commands.CreateProduct
{
    // comando que recibe el controller
    // devuelve Guid = nuevo Product.Id
    public sealed record CreateProductCommand(
        Guid VendorId,
        string Sku,
        string Name,
        string Description,
        string Slug,
        Guid CategoryId,
        decimal Price,
        int InitialStock,
        bool IsActive,
        string MainImageUrl,           // <--- NUEVO
        IReadOnlyList<string> ImageUrls // <--- NUEVO (lista de imágenes adicionales)
    ) : IRequest<Guid>;
}
