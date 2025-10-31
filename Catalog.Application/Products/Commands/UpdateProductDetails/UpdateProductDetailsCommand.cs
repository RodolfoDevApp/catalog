using System;
using MediatR;

namespace Catalog.Application.Products.Commands.UpdateProductDetails
{
    public sealed record UpdateProductDetailsCommand(
        Guid ProductId,
        string Name,
        string Description,
        string Slug,
        Guid CategoryId
    ) : IRequest<Unit>;
}
