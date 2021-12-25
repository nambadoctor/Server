using DataLayer;
using DataModel.Shared;
using FirebaseAdmin.Auth;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MiddleWare.Interfaces;
using NambaMiddleWare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NambaDoctorWebApi.Interceptors
{
    public class AuthInterceptor
    {
        private readonly RequestDelegate requestDelegate;
        private INDLogger ndLogger;
        private IAuthService authService;

        public AuthInterceptor(IAuthService authService, INDLogger ndLogger, RequestDelegate requestDelegate)
        {
            this.authService = authService;
            this.ndLogger = ndLogger;
            this.requestDelegate = requestDelegate;
        }

        [Authorize]
        public async Task Invoke(HttpContext context)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            ndLogger.LogEvent("ValidateTokenAndSetContext", SeverityLevel.Information);
            await ValidateTokenAndSetContext(context);

            timer.Stop();
            ndLogger.LogEvent($"UnaryServerHandler Execution time: {timer.ElapsedMilliseconds}", SeverityLevel.Information);
        }

        private async Task ValidateTokenAndSetContext(HttpContext context)
        {
            SetContextValues(context);

            ndLogger.LogEvent("Context values are set", SeverityLevel.Information);

            // Skip token validation for Ping API
            if (context.Request.Path.Equals("/api/Ping"))
            {
                await requestDelegate.Invoke(context);
                return;
            }

            bool isValid = false;

            var bearerToken = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                var segs = bearerToken.Split(" ");

                if (segs.Length == 2)
                {
                    var authToken = segs[1];

                    try
                    {
                        var decodedAuthToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(authToken);
                        NambaDoctorContext.FirebaseUserId = decodedAuthToken.Uid;

                        ndLogger.LogEvent($"FirebaseUserId: {decodedAuthToken.Uid}", SeverityLevel.Information);

                        decodedAuthToken.Claims.TryGetValue(Constants.Auth.PhoneNumber, out object phoneNumber);

                        if (phoneNumber != null)
                        {
                            NambaDoctorContext.PhoneNumber = phoneNumber.ToString();
                            ndLogger.LogEvent($"Authenticaed for Phone Number: {phoneNumber.ToString()}", SeverityLevel.Information);

                            //[TBD] - Check for blocked users
                            //[TBD] - Set TestUserType

                        }
                        else if (Constants.Auth.anonymousAllowList.Contains(context.GetEndpoint().DisplayName))
                        {
                            ndLogger.LogEvent($"Anonymous auth for valid api: {context.GetEndpoint().DisplayName}", SeverityLevel.Information);

                            /*[TBD: We are getting into call flows with out phone number set. 
                             * Will it have impact in database calls any other calls that has Phone Number depdency
                            */
                        }
                        else
                        {
                            ndLogger.LogEvent($"Anonymous auth for invalid api: {context.GetEndpoint().DisplayName}", SeverityLevel.Error);
                            throw new UnauthorizedAccessException("Unauthorized access not allowed");
                        }

                        ndLogger.LogEvent("SetUserInfo", SeverityLevel.Information);
                        await SetUserInfo();

                        if (!string.IsNullOrWhiteSpace(NambaDoctorContext.PhoneNumber))
                        {
                            isValid = true;
                        }

                        if (isValid)
                        {
                            await requestDelegate.Invoke(context);
                        }
                    }
                    catch (FirebaseAuthException)
                    {
                        isValid = false;
                    }
                }
            }

            if (isValid == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("User unauthorized");
                return;
            }
        }

        private void SetContextValues(HttpContext context)
        {
            NambaDoctorContext.ContextValues = StoreContextValues(context);
        }

        private Dictionary<string, string> StoreContextValues(HttpContext context)
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
            context.Request.Headers.TryGetValue("appointmentid", out var appointmentId);
            context.Request.Headers.TryGetValue("sessionid", out var sessionId);
            context.Request.Headers.TryGetValue("correlationid", out var correlationId);
            context.Request.Headers.TryGetValue("appversion", out var appVersion);
            context.Request.Headers.TryGetValue("eventmessage", out var eventMessage);
            context.Request.Headers.TryGetValue("loglevel", out var logLevel);
            context.Request.Headers.TryGetValue("isproduction", out var isProduction);
            context.Request.Headers.TryGetValue("deviceinfo", out var deviceInfo);
            context.Request.Headers.TryGetValue("eventtype", out var eventType);
            context.Request.Headers.TryGetValue("notificationmessageid", out var notificationMessageId);
            context.Request.Headers.TryGetValue("organisationid", out var organisationId);



            if (!string.IsNullOrWhiteSpace(organisationId))
            {
                traceDictionary.Add("OrganisationId", organisationId);
                NambaDoctorContext.OrganisationId = organisationId;
            }
            if (!string.IsNullOrWhiteSpace(appointmentId))
                traceDictionary.Add("AppointmentId", appointmentId);
            if (!string.IsNullOrWhiteSpace(sessionId))
                traceDictionary.Add("SessionId", sessionId);
            if (!string.IsNullOrWhiteSpace(correlationId))
                traceDictionary.Add("CorrelationId", correlationId);
            if (!string.IsNullOrWhiteSpace(appVersion))
                traceDictionary.Add("AppVersion", appVersion);
            if (!string.IsNullOrWhiteSpace(eventMessage))
                traceDictionary.Add("EventMessage", eventMessage);
            if (!string.IsNullOrWhiteSpace(logLevel))
                traceDictionary.Add("LogLevel", logLevel);
            if (!string.IsNullOrWhiteSpace(isProduction))
                traceDictionary.Add("IsProduction", isProduction);
            if (!string.IsNullOrWhiteSpace(deviceInfo))
                traceDictionary.Add("DeviceInfo", deviceInfo);
            if (!string.IsNullOrWhiteSpace(eventType))
                traceDictionary.Add("EventType", eventType);
            if (!string.IsNullOrWhiteSpace(notificationMessageId))
                traceDictionary.Add("NotificationMessageId", notificationMessageId);

            return traceDictionary;
        }

        private async Task SetUserInfo()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                var customer = await authService.GetCustomerFromRegisteredPhoneNumber();

                if (customer != null)
                {
                    ndLogger.LogEvent($"Setting Customer Id :{customer.CustomerId.ToString()}");

                    NambaDoctorContext.NDUserId = customer.CustomerId.ToString();
                    NambaDoctorContext.ndUserType = NDUserType.Customer;
                    NambaDoctorContext.Designation = "Customer";

                    return;
                }

                var serviceProvider = await authService.GetServiceProviderFromRegisteredPhoneNumber();

                if (serviceProvider != null)
                {
                    ndLogger.LogEvent($"Setting serviceProvider Id :{serviceProvider.ServiceProviderId.ToString()}");

                    NambaDoctorContext.NDUserId = serviceProvider.ServiceProviderId.ToString();
                    NambaDoctorContext.ndUserType = NDUserType.ServiceProvider;
                    if (string.IsNullOrWhiteSpace(NambaDoctorContext.OrganisationId))
                    {
                        ndLogger.LogEvent("SetDefaultOrganisation", SeverityLevel.Information);
                        await SetDefaultOrganisation();
                    }
                    NambaDoctorContext.Designation = serviceProvider.Profiles.Find(
                        profile => profile.OrganisationId == NambaDoctorContext.OrganisationId)
                        .ServiceProviderType;

                    return;
                }

                ndLogger.LogEvent("Setting User Not registered");
                NambaDoctorContext.ndUserType = NDUserType.NotRegistered;
                NambaDoctorContext.Designation = "NotRegistered";
            }
            finally
            {
                timer.Stop();
                ndLogger.LogEvent($"SetUserInfo Execution time: {timer.ElapsedMilliseconds.ToString()}", SeverityLevel.Information);

            }

        }

        private async Task SetDefaultOrganisation()
        {
            NambaDoctorContext.OrganisationId = await authService.GetDefaultOrganisationId();
        }

    }
}
