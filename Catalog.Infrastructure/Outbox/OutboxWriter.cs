using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;

namespace Catalog.Infrastructure.Outbox
{
    // OutboxWriter NO publica a RabbitMQ directamente.
    // Solo inserta una fila en la tabla outbox_messages
    // con el JSON del evento de integración.
    //
    // Ese JSON luego será leído por el OutboxDispatcherHostedService,
    // que sí usará IEventBus y RabbitMQ.
    internal sealed class OutboxWriter : IOutboxWriter
    {
        private readonly CatalogDbContext _db;

        public OutboxWriter(CatalogDbContext db)
        {
            _db = db;
        }

        public async Task AddMessageAsync(
            string type,
            object payload,
            DateTime occurredAtUtc,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Outbox message type is required", nameof(type));

            if (occurredAtUtc == default)
                throw new ArgumentException("OccurredAtUtc is required", nameof(occurredAtUtc));

            // Serializamos el record del IntegrationEvent (ej. ProductCreatedIntegrationEvent)
            var json = JsonSerializer.Serialize(payload,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

            var msg = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                PayloadJson = json,
                OccurredAtUtc = occurredAtUtc,
                ProcessedAtUtc = null,
                RetryCount = 0
            };

            await _db.OutboxMessages.AddAsync(msg, ct);
            // IMPORTANTE: no guardamos aquí. Ese SaveChangesAsync
            // lo hace el repositorio cuando termine el caso de uso.
        }
    }
}
