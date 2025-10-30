using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Hosting
{
    // HostedService que corre en background y llama al OutboxDispatcher periódicamente.
    // IMPORTANTE:
    //   - HostedService es singleton.
    //   - OutboxDispatcher es scoped (porque usa DbContext).
    //   - Para no mezclar lifetimes, creamos un scope en cada iteración.
    internal sealed class OutboxDispatcherHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcherHostedService> _logger;

        // cada cuánto drenar la outbox
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

        public OutboxDispatcherHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxDispatcherHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox dispatcher background worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Creamos un scope nuevo por iteración
                    using var scope = _scopeFactory.CreateScope();

                    // Resolvemos OutboxDispatcher dentro de ese scope
                    var dispatcher = scope
                        .ServiceProvider
                        .GetRequiredService<OutboxDispatcher>();

                    // Ejecutamos el drenado de mensajes pendientes
                    var processed = await dispatcher
                        .DispatchPendingMessagesAsync(stoppingToken);

                    if (processed > 0)
                    {
                        _logger.LogInformation(
                            "Outbox dispatcher processed {Count} messages.",
                            processed
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Outbox dispatcher loop error. Will retry next tick."
                    );
                }

                // esperamos el próximo ciclo
                await Task.Delay(Interval, stoppingToken);
            }

            _logger.LogInformation("Outbox dispatcher background worker stopping.");
        }
    }
}
