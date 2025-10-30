using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Outbox;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Messaging.RabbitMQ.DependencyInjection; // tu paquete
using Messaging.RabbitMQ.Bus;                // RabbitMqOptions

namespace Catalog.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. EF Core / Postgres
            var connectionString = configuration.GetConnectionString("CatalogDb");

            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            // 2. Repositorios y outbox writer (para los command handlers)
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOutboxWriter, OutboxWriter>();

            // 3. Dispatcher que sabe drenar la outbox
            services.AddScoped<OutboxDispatcher>();

            // 4. Background worker que corre el dispatcher en loop
            services.AddHostedService<OutboxDispatcherHostedService>();

            // 5. Registrar RabbitMQ Event Bus de tu librería compartida
            services.AddMessagingRabbitMq(opts =>
            {
                opts.ServiceName = "catalog";

                // Como tu API corre en Windows host y RabbitMQ está en Docker
                // escuchando el port publicado 5672 -> usa "localhost"
                opts.HostName = "host.docker.internal";
                opts.Port = 5672;
                opts.UserName = "admin";
                opts.Password = "admin";
                opts.VirtualHost = "/";

                opts.PrefetchCount = 16;
                opts.MaxRetryAttempts = 5;
                opts.RetryDelayMilliseconds = 5000;
            });

            return services;
        }
    }
}
