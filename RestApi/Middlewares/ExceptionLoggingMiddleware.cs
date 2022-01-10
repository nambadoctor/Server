using DataModel.Shared;
using DataModel.Shared.Exceptions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionLoggingMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (AppointmentDoesNotExistException ex)
            {
                logger.LogError(ex, "Appointment Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 501);
            }
            catch (ServiceRequestDoesNotExistException ex)
            {
                logger.LogError(ex, "ServiceRequest Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 502);
            }
            catch (CustomerDoesNotExistException ex)
            {
                logger.LogError(ex, "Customer Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 503);
            }
            catch (ServiceProviderDoesnotExistsException ex)
            {
                logger.LogError(ex, "ServiceProvider Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 504);
            }
            catch (PrescriptionDoesNotExistException ex)
            {
                logger.LogError(ex, "Prescription Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 505);
            }
            catch (ReportDoesNotExistException ex)
            {
                logger.LogError(ex, "Report Does Not Exist");
                throw new BadHttpRequestException(ex.Message, 506);
            }
            catch (PhoneNumberBelongsToServiceProviderException ex)
            {
                logger.LogError(ex, "Phone number already in use");
                throw new BadHttpRequestException(ex.Message, 507);
            }
            catch (IOException ex)
            {
                logger.LogError(ex, "Error reading or writing to blob");
                throw new BadHttpRequestException(ex.Message, 508);
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex, "Invalid argument passed");
                throw new BadHttpRequestException(ex.Message, 401);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, "Key not found in collection");
                throw new BadHttpRequestException(ex.Message, 404);
            }
            catch (NullReferenceException ex)
            {
                logger.LogError(ex, "Null object encountered");
                throw new BadHttpRequestException(ex.Message, 500);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
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
