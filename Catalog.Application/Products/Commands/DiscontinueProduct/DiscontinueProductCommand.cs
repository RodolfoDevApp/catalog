using System;
using MediatR;

namespace Catalog.Application.Products.Commands.DiscontinueProduct
{
    // "Delete" de dominio: marca IsDiscontinued = true y IsActive = false
    public sealed record DiscontinueProductCommand(Guid ProductId) : IRequest<Unit>;
}
