using DataModel.Shared;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApi.Middlewares
{
    public class UserinfoMiddleware
    {
        private readonly RequestDelegate _next;
        public UserinfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            foreach (Claim claim in context.User.Claims)
            {
                if (claim.Type == "phone_number")
                {
                    NambaDoctorContext.PhoneNumber = claim.Value;
                    NambaDoctorContext.AddTraceContext("phoneNumber", claim.Value);
                }
            }
            await _next(context);
        }

    }
}
