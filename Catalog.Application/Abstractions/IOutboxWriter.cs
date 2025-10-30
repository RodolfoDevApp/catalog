using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Abstractions
{
    // Puerto para insertar mensajes en la tabla outbox_messages.
    // Implementado en Infrastructure.Outbox.OutboxWriter
    public interface IOutboxWriter
    {
        Task AddMessageAsync(
            string type,
            object payload,
            DateTime occurredAtUtc,
            CancellationToken ct);
    }
}
