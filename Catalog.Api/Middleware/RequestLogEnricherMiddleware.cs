using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog.Context;
using System.Net.Http;
using System.Threading.Tasks;

namespace Catalog.Api.Middleware
{
    public class RequestLogEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _serviceName;

        public RequestLogEnricherMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _serviceName = config["ServiceName"] ?? "catalog-api";
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = context.TraceIdentifier;
            var path = context.Request.Path.Value ?? string.Empty;
            var method = context.Request.Method;

            // Estas propiedades quedan "activas" mientras dure la request
            using (LogContext.PushProperty("service", _serviceName))
            using (LogContext.PushProperty("traceId", traceId))
            using (LogContext.PushProperty("path", path))
            using (LogContext.PushProperty("method", method))
            {
                await _next(context);
            }
        }
    }
}
