using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Catalog.Infrastructure.Persistence.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Tabla y PK
            builder.ToTable("products");
            builder.HasKey(p => p.Id);

            // Id
            builder.Property(p => p.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            // VendorId
            builder.Property(p => p.VendorId)
                .HasColumnName("vendor_id")
                .IsRequired();

            // SKU
            builder.Property(p => p.Sku)
                .HasColumnName("sku")
                .IsRequired()
                .HasMaxLength(64);

            // Name
            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            // Description
            builder.Property(p => p.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(2000);

            // Slug
            builder.Property(p => p.Slug)
                .HasColumnName("slug")
                .IsRequired()
                .HasMaxLength(200);

            // CategoryId
            builder.Property(p => p.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            // Price
            builder.Property(p => p.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            // StockQuantity
            builder.Property(p => p.StockQuantity)
                .HasColumnName("stock_quantity")
                .IsRequired();

            // MainImageUrl (string)
            builder.Property(p => p.MainImageUrl)
                .HasColumnName("main_image_url")
                .HasMaxLength(500); // ajusta si quieres más largo

            // ImageUrls (lista de strings) -> jsonb en Postgres
            builder.Property(p => p.ImageUrls)
                .HasColumnName("image_urls")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
                );

            // IsActive
            builder.Property(p => p.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            // IsDiscontinued
            builder.Property(p => p.IsDiscontinued)
                .HasColumnName("is_discontinued")
                .IsRequired();

            // CreatedAtUtc
            builder.Property(p => p.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            // UpdatedAtUtc
            builder.Property(p => p.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            // Índices
            builder.HasIndex(p => p.Sku)
                .IsUnique()
                .HasDatabaseName("ux_products_sku");

            builder.HasIndex(p => p.Slug)
                .IsUnique()
                .HasDatabaseName("ux_products_slug");

            builder.HasIndex(p => p.VendorId)
                .HasDatabaseName("ix_products_vendor_id");

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("ix_products_category_id");
        }
    }
}
