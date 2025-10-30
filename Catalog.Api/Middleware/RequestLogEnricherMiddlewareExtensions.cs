using Catalog.Api.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Catalog.Api.Middleware
{
    public static class RequestLogEnricherMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogEnricher(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogEnricherMiddleware>();
        }
    }
}
