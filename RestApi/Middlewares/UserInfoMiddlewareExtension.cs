using Microsoft.AspNetCore.Builder;

namespace RestApi.Middlewares
{
    public static class UserInfoMiddlewareExtension
    {
        public static IApplicationBuilder UseUserContextSet(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserinfoMiddleware>();
        }
    }
}
