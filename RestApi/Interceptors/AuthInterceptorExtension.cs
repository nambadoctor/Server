using Microsoft.AspNetCore.Builder;

namespace NambaDoctorWebApi.Interceptors
{
    public static class AuthInterceptorExtension
    {
        public static IApplicationBuilder ValidateToken(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthInterceptor>();
            return app;
        }
    }
}
