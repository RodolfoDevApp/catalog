using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Infrastructure.Persistence;
using Messaging.Core.Abstractions; // IEventBus de tu librería
using Messaging.Core.Primitives;  // Event base, IntegrationEventEnvelope
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Outbox
{
    internal sealed class OutboxDispatcher
    {
        private readonly CatalogDbContext _db;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OutboxDispatcher> _logger;

        // Cuántos mensajes drenamos por ciclo
        private const int BatchSize = 20;

        // Límite de reintentos antes de rendirse
        private const int MaxRetryBeforeGiveUp = 5;

        public OutboxDispatcher(
            CatalogDbContext db,
            IEventBus eventBus,
            ILogger<OutboxDispatcher> logger)
        {
            _db = db;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<int> DispatchPendingMessagesAsync(CancellationToken ct)
        {
            // 1. Traer mensajes que todavía no se procesaron
            var pending = await _db.OutboxMessages
                .Where(m =>
                    m.ProcessedAtUtc == null &&
                    m.RetryCount <= MaxRetryBeforeGiveUp)
                .OrderBy(m => m.OccurredAtUtc)
                .Take(BatchSize)
                .ToListAsync(ct);

            if (!pending.Any())
            {
                return 0;
            }

            foreach (var msg in pending)
            {
                try
                {
                    _logger.LogInformation(
                        "[Outbox] Publishing {Type} Id={OutboxId} Retry={Retry}",
                        msg.Type,
                        msg.Id,
                        msg.RetryCount
                    );

                    // Este objeto es el "sobre" genérico que viaja por RabbitMQ
                    var envelope = new IntegrationEventEnvelope(
                        type: msg.Type,
                        payloadJson: msg.PayloadJson,
                        occurredAtUtc: msg.OccurredAtUtc
                    );

                    // Publicar al bus (RabbitMQ) usando tu IEventBus
                    await _eventBus.PublishAsync(envelope, ct);

                    // Marcamos como procesado con timestamp
                    msg.ProcessedAtUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Falló el publish: aumentamos RetryCount y NO ponemos ProcessedAtUtc
                    _logger.LogError(
                        ex,
                        "[Outbox] Error publishing {Type} OutboxId={OutboxId}",
                        msg.Type,
                        msg.Id
                    );

                    msg.RetryCount += 1;
                }
            }

            // Guardar todos los cambios (tanto procesados como retries)
            await _db.SaveChangesAsync(ct);

            return pending.Count;
        }
    }
}
