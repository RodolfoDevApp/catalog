using System;
using System.Collections.Generic;

namespace Catalog.Domain.Products
{
    public sealed class Product
    {
        // Identidad
        public Guid Id { get; private set; }
        public Guid VendorId { get; private set; }

        // Datos comerciales
        public string Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public string Slug { get; private set; } = default!;
        public Guid CategoryId { get; private set; }

        // Imágenes
        public string MainImageUrl { get; private set; } = default!;            // portada / cover
        public List<string> ImageUrls { get; private set; } = new();            // galería adicional

        // Precio e inventario
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }

        // Estado ciclo de vida
        public bool IsActive { get; private set; }
        public bool IsDiscontinued { get; private set; }

        // Auditoría
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        private Product() { }

        public static Product Create(
            Guid vendorId,
            string sku,
            string name,
            string description,
            string slug,
            Guid categoryId,
            decimal price,
            int initialStock,
            bool isActive,
            string mainImageUrl,
            IEnumerable<string> imageUrls,
            DateTime nowUtc)
        {
            if (vendorId == Guid.Empty)
                throw new ArgumentException("Vendor id is required", nameof(vendorId));
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU is required", nameof(sku));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category id is required", nameof(categoryId));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));
            if (initialStock < 0)
                throw new ArgumentException("Initial stock cannot be negative", nameof(initialStock));
            if (nowUtc == default)
                throw new ArgumentException("Creation timestamp is required", nameof(nowUtc));
            if (string.IsNullOrWhiteSpace(mainImageUrl))
                throw new ArgumentException("Main image url is required", nameof(mainImageUrl));

            var product = new Product
            {
                Id = Guid.NewGuid(),
                VendorId = vendorId,
                Sku = sku.Trim(),
                Name = name.Trim(),
                Description = description.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                CategoryId = categoryId,

                MainImageUrl = mainImageUrl.Trim(),
                ImageUrls = new List<string>(imageUrls ?? Array.Empty<string>()),

                Price = price,
                StockQuantity = initialStock,
                IsActive = isActive,
                IsDiscontinued = false,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = null
            };

            return product;
        }

        public void UpdateDetails(
            string name,
            string description,
            string slug,
            Guid categoryId,
            string mainImageUrl,
            IEnumerable<string> imageUrls,
            DateTime nowUtc)
        {
            if (IsDiscontinued)
                throw new InvalidOperationException("Cannot update a discontinued product.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug is required", nameof(slug));
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category id is required", nameof(categoryId));
            if (nowUtc == default)
                throw new ArgumentException("Update timestamp is required", nameof(nowUtc));
            if (string.IsNullOrWhiteSpace(mainImageUrl))
                throw new ArgumentException("Main image url is required", nameof(mainImageUrl));

            Name = name.Trim();
            Description = description.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            CategoryId = categoryId;

            MainImageUrl = mainImageUrl.Trim();
            ImageUrls = new List<string>(imageUrls ?? Array.Empty<string>());

            UpdatedAtUtc = nowUtc;
        }

        public void ChangePrice(decimal newPrice, DateTime nowUtc)
        {
            if (IsDiscontinued)
                throw new InvalidOperationException("Cannot change price of a discontinued product.");
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPrice));
            if (nowUtc == default)
                throw new ArgumentException("Update timestamp is required", nameof(nowUtc));

            Price = newPrice;
            UpdatedAtUtc = nowUtc;
        }

        public void AdjustStock(int deltaQuantity, DateTime nowUtc)
        {
            if (IsDiscontinued)
                throw new InvalidOperationException("Cannot adjust stock of a discontinued product.");
            if (deltaQuantity == 0)
                throw new ArgumentException("DeltaQuantity cannot be zero.", nameof(deltaQuantity));
            if (nowUtc == default)
                throw new ArgumentException("Update timestamp is required", nameof(nowUtc));

            var newStock = StockQuantity + deltaQuantity;
            if (newStock < 0)
                throw new InvalidOperationException("Stock cannot go below zero.");

            StockQuantity = newStock;
            UpdatedAtUtc = nowUtc;
        }

        public void Deactivate(DateTime nowUtc)
        {
            if (IsDiscontinued)
                throw new InvalidOperationException("Product is already discontinued.");
            if (nowUtc == default)
                throw new ArgumentException("Update timestamp is required", nameof(nowUtc));

            IsActive = false;
            UpdatedAtUtc = nowUtc;
        }

        public void Discontinue(DateTime nowUtc)
        {
            if (nowUtc == default)
                throw new ArgumentException("Update timestamp is required", nameof(nowUtc));

            IsActive = false;
            IsDiscontinued = true;
            UpdatedAtUtc = nowUtc;
        }
    }
}
