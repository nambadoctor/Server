using DataModel.Shared;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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

        public async Task InvokeAsync(HttpContext context)
        {
            StoreValuesFromContext(context);
            await _next(context);
        }
        private void StoreValuesFromContext(HttpContext context)
        {
            var traceDictionary = new Dictionary<string, string>();

            if (context.Request.Headers.TryGetValue("userid", out var userId))
            {
                traceDictionary.Add("UserId", userId);
            }

            if (context.Request.Headers.TryGetValue("usertype", out var userType))
            {
                traceDictionary.Add("UserType", userType);

            }

            if (context.Request.Headers.TryGetValue("eventdatetime", out var eventDateTime))
            {
                traceDictionary.Add("EventDateTime", eventDateTime);
            }

            if (context.Request.Headers.TryGetValue("appointmentid", out var appointmentId))
            {
                traceDictionary.Add("AppointmentId", appointmentId);
            }

            if (context.Request.Headers.TryGetValue("sessionid", out var sessionId))
            {
                traceDictionary.Add("sessionid", sessionId);

            }

            if (context.Request.Headers.TryGetValue("correlationid", out var correlationId))
            {
                traceDictionary.Add("correlationid", correlationId);

            }

            if (context.Request.Headers.TryGetValue("appversion", out var appversion))
            {
                traceDictionary.Add("appversion", appversion);
            }

            if (context.Request.Headers.TryGetValue("eventmessage", out var eventMessage))
            {
                traceDictionary.Add("eventmessage", eventMessage);
            }

            if (context.Request.Headers.TryGetValue("loglevel", out var logLevel))
            {
                traceDictionary.Add("loglevel", logLevel);
            }

            if (context.Request.Headers.TryGetValue("isproduction", out var isProduction))
            {
                traceDictionary.Add("isproduction", isProduction);
            }

            if (context.Request.Headers.TryGetValue("deviceinfo", out var deviceInfo))
            {
                traceDictionary.Add("deviceinfo", deviceInfo);
            }

            if (context.Request.Headers.TryGetValue("eventtype", out var eventType))
            {
                traceDictionary.Add("eventtype", eventType);
            }

            if (context.Request.Headers.TryGetValue("notificationmessageid", out var notificationMessageId))
            {
                traceDictionary.Add("notificationmessageid", notificationMessageId);
            }

            if (context.Request.Headers.TryGetValue("organisationid", out var organisationId))
            {
                traceDictionary.Add("OrganisationId", organisationId);
            }

            NambaDoctorContext.TraceContextValues = traceDictionary;

        }

    }
}
