using DataModel.Shared;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApi.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private INDLogger ndLogger;

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, INDLogger ndLogger)
        {
            this.ndLogger = ndLogger;
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                ndLogger.LogEvent($"Invalid parameters passed: {ex.Message} {ex.StackTrace}", SeverityLevel.Error);
                throw new BadHttpRequestException(ex.Message, 401);
            }
            catch (KeyNotFoundException ex)
            {
                ndLogger.LogEvent($"No object found in collection: {ex.Message} {ex.StackTrace}", SeverityLevel.Error);
                throw new BadHttpRequestException(ex.Message, 404);
            }
            catch (NullReferenceException ex)
            {
                ndLogger.LogEvent($"Operation on null object: {ex.Message} {ex.StackTrace}", SeverityLevel.Error);
                throw new BadHttpRequestException(ex.Message, 404);
            }
            catch (Exception ex)
            {
                ndLogger.LogEvent($"Uncaught error: {ex.Message} {ex.StackTrace}", SeverityLevel.Error);
                throw new BadHttpRequestException(ex.Message, 500); ;
            }
        }

        /*Status codes to remember
            400: Bad Request
            401: Unauthorized
            402: Payment Required
            403: Access forbidden
            404: Not found exception
            500: Unhandled
         */
    }
}
