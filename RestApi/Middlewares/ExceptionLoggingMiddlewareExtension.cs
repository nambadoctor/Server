using Microsoft.AspNetCore.Builder;

namespace RestApi.Middlewares
{
    public static class ExceptionLoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseExceptionLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionLoggingMiddleware>();
        }
    }
}
