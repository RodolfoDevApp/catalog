using System;

namespace Catalog.Api.Contracts
{
    public sealed record UpdateProductDetailsRequest(
        string Name,
        string Description,
        string Slug,
        Guid CategoryId
    );
}
