using System;
using System.Collections.Generic;

namespace Catalog.Api.Contracts
{
    // Contrato HTTP del POST /products
    // Esto es lo que el cliente (dashboard admin, etc.) manda.
    public sealed class CreateProductRequest
    {
        public Guid VendorId { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public decimal Price { get; set; }
        public int InitialStock { get; set; }
        public bool IsActive { get; set; }

        // URL de la imagen principal (cover)
        public string MainImageUrl { get; set; } = default!;          // <- no nullable

        // Galería de imágenes adicionales
        public List<string> ImageUrls { get; set; } = new();
    }
}
