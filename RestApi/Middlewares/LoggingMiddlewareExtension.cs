using Microsoft.AspNetCore.Builder;

namespace RestApi.Middlewares
{
    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLogContextSet(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
