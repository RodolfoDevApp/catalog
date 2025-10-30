using Catalog.Api.Middleware;
using Catalog.Application; // para AssemblyReference
using Catalog.Infrastructure.DependencyInjection;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);



// 1. Config de Serilog ANTES de construir el app
//    Importante: escribimos en JSON a consola, porque Promtail va a leer stdout del contenedor.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    // ?? Esta es la forma compatible
    .WriteTo.Console(new JsonFormatter(renderMessage: true))
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Infraestructura completa (DbContext, repos, outbox)
builder.Services.AddCatalogInfrastructure(builder.Configuration);
// MediatR (CQRS dentro del microservicio)
// Le decimos "busca handlers en el assembly de Application"
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
});
// También: serviceName en config para el enricher
builder.Configuration["ServiceName"] = "catalog-api";

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.Migrate(); // aplica todas las migraciones pendientes
}
// Enriquecemos contexto de log con info del request
app.UseRequestLogEnricher();

// Manejo global de excepciones + respuesta RFC7807-like
app.UseGlobalExceptionHandling();

// Métricas automáticas de HTTP (status code, duración, etc.)
app.UseHttpMetrics();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "catalog" }));


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// <<< ESTE ENDPOINT LO LEE PROMETHEUS
app.MapMetrics(); // publica /metrics

app.Run();
