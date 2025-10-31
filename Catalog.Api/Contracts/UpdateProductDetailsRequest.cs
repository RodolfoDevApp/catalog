using System;

namespace Catalog.Api.Contracts
{
    public sealed class UpdateProductDetailsRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}
