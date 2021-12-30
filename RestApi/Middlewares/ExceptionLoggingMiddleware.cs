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

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                throw new BadHttpRequestException(ex.Message, 401);
            }
            catch (KeyNotFoundException ex)
            {
                throw new BadHttpRequestException(ex.Message, 404);
            }
            catch (NullReferenceException ex)
            {
                throw new BadHttpRequestException(ex.Message, 404);
            }
            catch (Exception ex)
            {
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
