using System.Text.Json;
using Catalog.Application.Common.Exceptions; // <-- IMPORTANTE
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, title) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                    ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                    ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
                    _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
                };

                var traceId = context.TraceIdentifier;

                _logger.LogError(ex,
                    "Error procesando {Method} {Path} => {StatusCode}. TraceId={TraceId}. ExceptionType={ExceptionType}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    traceId,
                    ex.GetType().Name
                );

                var problem = new
                {
                    traceId,
                    title,
                    detail = ex.Message,
                    status = statusCode,
                    instance = context.Request.Path.Value,
                    service = "catalog-api"
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
