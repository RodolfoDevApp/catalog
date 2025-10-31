using System;
using MediatR;

namespace Catalog.Application.Products.Queries.GetProductById
{
    // Query simple por Id. El handler devolvera un DTO local
    public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<GetProductByIdResult>;
}
