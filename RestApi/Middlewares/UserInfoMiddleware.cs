using DataModel.Shared;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using MiddleWare.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApi.Middlewares
{
    public class UserinfoMiddleware
    {
        private readonly RequestDelegate _next;
        private INDLogger ndLogger;
        private IAuthService authService;
        public UserinfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService, INDLogger ndLogger)
        {
            this.authService = authService;
            this.ndLogger = ndLogger;

            foreach (Claim claim in context.User.Claims)
            {
                if(claim.Type == "phone_number")
                {
                    NambaDoctorContext.PhoneNumber = claim.Value;
                }
            }
            await _next(context);
        }

    }
}
