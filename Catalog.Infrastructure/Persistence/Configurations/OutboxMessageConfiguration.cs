using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations
{
    // Mapeo de la tabla técnica "outbox_messages".
    // Aquí viven los mensajes pendientes de publicar a RabbitMQ.
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName("id")
                .ValueGeneratedNever(); // igual que Product, usaremos Guid manual

            builder.Property(o => o.Type)
                .HasColumnName("type")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.PayloadJson)
                .HasColumnName("payload_json")
                .IsRequired();

            builder.Property(o => o.OccurredAtUtc)
                .HasColumnName("occurred_at_utc")
                .IsRequired();

            builder.Property(o => o.ProcessedAtUtc)
                .HasColumnName("processed_at_utc");

            builder.Property(o => o.RetryCount)
                .HasColumnName("retry_count")
                .IsRequired();

            // Índice para que el dispatcher encuentre rápido "pendientes"
            // (ProcessedAtUtc == null) y las más viejas primero.
            builder.HasIndex(o => new { o.ProcessedAtUtc, o.OccurredAtUtc })
                .HasDatabaseName("ix_outbox_messages_pending");
        }
    }
}
