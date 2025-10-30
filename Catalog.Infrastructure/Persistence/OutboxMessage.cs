using System;

namespace Catalog.Infrastructure.Persistence
{
    // Esta entidad representa una fila en la tabla "outbox_messages".
    //
    // Flujo:
    // 1) Durante el manejo de un command (CreateProduct, etc.)
    //    además de guardar el Product, insertamos un OutboxMessage
    //    con el payload del evento de integración.
    //
    // 2) Más tarde, un HostedService (OutboxDispatcherHostedService)
    //    lee estas filas pendientes, las publica a RabbitMQ a través de IEventBus,
    //    y las marca como procesadas.
    //
    // Esto nos da entrega "at-least-once" sin perder mensajes si el proceso cae.
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }

        // Nombre lógico del evento de integración
        // Ej: "ProductCreated", "ProductPriceChanged"
        public string Type { get; set; } = default!;

        // JSON serializado del evento de integración (el record que vive en Application/IntegrationEvents)
        public string PayloadJson { get; set; } = default!;

        // Cuándo ocurrió realmente en el dominio
        public DateTime OccurredAtUtc { get; set; }

        // Cuándo logramos publicarlo a RabbitMQ (null => pendiente)
        public DateTime? ProcessedAtUtc { get; set; }

        // Para reintentos del dispatcher
        public int RetryCount { get; set; }
    }
}
