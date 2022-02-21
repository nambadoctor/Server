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
            logger.LogInformation($"Request: {await GetRequestString(context.Request)}");
            StoreValuesFromContext(context);
            logger.LogInformation($"Response: {Truncate(await GetResponseString(context.Response), 50)}");
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

        public async Task<string> GetRequestString(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body = body;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        public async Task<string> GetResponseString(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }

        public string Truncate(string variable, int Length)
        {
            if (string.IsNullOrEmpty(variable)) return variable;
            return variable.Length <= Length ? variable : variable.Substring(0, Length);
        }

    }
}
