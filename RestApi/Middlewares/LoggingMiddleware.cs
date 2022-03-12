using DataModel.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestApi.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LoggingMiddleware> logger)
        {
            logger.LogInformation($"Request: {context.Request}");
            StoreValuesFromContext(context);
            logger.LogInformation($"Response: {context.Response}");
            await _next(context);
        }
        private void StoreValuesFromContext(HttpContext context)
        {
            var traceDictionary = new Dictionary<string, object>();

            if (context.Request.Headers.TryGetValue("phn", out var phonenumber))
            {
                traceDictionary.Add("phonenumber", phonenumber);
            }

            if (context.Request.Headers.TryGetValue("orgId", out var orgId))
            {
                traceDictionary.Add("OrganisationId", orgId);
            }

            if (context.Request.Headers.TryGetValue("spid", out var spid))
            {
                traceDictionary.Add("ServiceProviderId", spid);
            }

            if (context.Request.Headers.TryGetValue("sptype", out var sptype))
            {
                traceDictionary.Add("ServiceProviderType", sptype);
            }

            if (context.Request.Headers.TryGetValue("spname", out var spname))
            {
                traceDictionary.Add("ServiceProviderName", spname);
            }

            if (context.Request.Headers.TryGetValue("cv", out var appversion))
            {
                traceDictionary.Add("clientVersion", appversion);
            }

            if (context.Request.Headers.TryGetValue("deviceinfo", out var deviceInfo))
            {
                traceDictionary.Add("deviceinfo", deviceInfo);
            }



            NambaDoctorContext.TraceContextValues = traceDictionary;

        }

    }
}
