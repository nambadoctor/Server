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
using System.IO;
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

            StoreValuesFromContext(context);
            ndLogger.LogEvent("Trace Context values are set", SeverityLevel.Information);

            ndLogger.LogEvent("ValidateTokenAndSetContext", SeverityLevel.Information);
            await ValidateTokenAndSetContext(context);

            await SetUserInfo();

            timer.Stop();
            ndLogger.LogEvent($"UnaryServerHandler Execution time: {timer.ElapsedMilliseconds}", SeverityLevel.Information);
        }


        private async Task ValidateTokenAndSetContext(HttpContext context)
        {
            FirebaseToken decodedAuthToken = null;
            bool isvalidUser = false;
            string errorReason = string.Empty;

            ndLogger.LogEvent("Reading Authorization token", SeverityLevel.Information);

            if (context.Request.Headers.TryGetValue("Authorization", out var bearerToken))
            {
                ndLogger.LogEvent($"bearerToken: {bearerToken.ToString()}", SeverityLevel.Information);

                var segs = bearerToken.ToString().Split(Constants.Auth.Spliter);

                if (segs.Length == 2)
                {
                    var authToken = segs[1];
                    ndLogger.LogEvent($"authToken: {authToken}", SeverityLevel.Information);

                    try
                    {
                        decodedAuthToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(authToken);

                        NambaDoctorContext.FirebaseUserId = decodedAuthToken.Uid;
                        ndLogger.LogEvent($"FirebaseUserId: {decodedAuthToken.Uid}", SeverityLevel.Information);

                        decodedAuthToken.Claims.TryGetValue(Constants.Auth.PhoneNumber, out object phoneNumber);

                        if (phoneNumber != null)
                        {
                            NambaDoctorContext.PhoneNumber = phoneNumber.ToString();
                            ndLogger.LogEvent($"Authenticaed for Phone Number: {phoneNumber.ToString()}", SeverityLevel.Information);
                            isvalidUser = true;
                        }
                        else if (Constants.Auth.anonymousAllowList.Contains(context.GetEndpoint().DisplayName))
                        {
                            ndLogger.LogEvent($"Anonymous auth for valid api: {context.GetEndpoint().DisplayName}", SeverityLevel.Information);
                            errorReason = "AnonymousAccessNotAllowed";

                            /*[TBD: We are getting into call flows with out phone number set. 
                             * Will it have impact in database calls any other calls that has Phone Number depdency
                            */
                        }

                    }
                    catch(FirebaseAuthException fbauthex)
                    {
                        errorReason = "FirebaseAuthException";
                        ndLogger.LogEvent($"VerifyAndGetAuthToken: Firebase Auth exception: {fbauthex.ToString}", SeverityLevel.Error);
                    }
                    catch (Exception ex)
                    {
                        errorReason = "UnknownException";
                        ndLogger.LogEvent($"Exception in VerifyIdTokenAsync: {ex.ToString}", SeverityLevel.Error);
                    }
                }
                else
                {
                    errorReason = "Invalid bearer token";
                    ndLogger.LogEvent("VerifyAndGetAuthToken: Invalid bearer token", SeverityLevel.Error);
                }
            }
            else
            {
                errorReason = "Invalid Authorization header";

                ndLogger.LogEvent("VerifyAndGetAuthToken: Invalid Authorization header", SeverityLevel.Error);
            }

            if(!isvalidUser)
            {
                ndLogger.LogEvent("VerifyAndGetAuthToken: Invalid Authorization header", SeverityLevel.Error);

                throw new UnauthorizedAccessException(errorReason);
            }
        }

        private void  StoreValuesFromContext(HttpContext context)
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

            if(context.Request.Headers.TryGetValue("appointmentid", out var appointmentId))
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

            if(context.Request.Headers.TryGetValue("loglevel", out var logLevel))
            {
                traceDictionary.Add("loglevel", logLevel);
            }

            if(context.Request.Headers.TryGetValue("isproduction", out var isProduction))
            {
                traceDictionary.Add("isproduction", isProduction);
            }
            
            if(context.Request.Headers.TryGetValue("deviceinfo", out var deviceInfo))
            {
                traceDictionary.Add("deviceinfo", deviceInfo);
            }

            if(context.Request.Headers.TryGetValue("eventtype", out var eventType))
            {
                traceDictionary.Add("eventtype", eventType);
            }

            if(context.Request.Headers.TryGetValue("notificationmessageid", out var notificationMessageId))
            {
                traceDictionary.Add("notificationmessageid", notificationMessageId);
            }

            if(context.Request.Headers.TryGetValue("organisationid", out var organisationId))
            {
                traceDictionary.Add("OrganisationId", organisationId);
                NambaDoctorContext.OrganisationId = organisationId;

            }

            NambaDoctorContext.TraceContextValues = traceDictionary;

        }

        private async Task SetUserInfo()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {

                var serviceProvider = await authService.GetServiceProviderFromRegisteredPhoneNumber();

                if (serviceProvider != null)
                {
                    ndLogger.LogEvent($"Setting serviceProvider Id :{serviceProvider.ServiceProviderId.ToString()}", SeverityLevel.Information);


                    NambaDoctorContext.NDUserId = serviceProvider.ServiceProviderId.ToString();
                    NambaDoctorContext.ndUserType = NDUserType.ServiceProvider;

                    if (!string.IsNullOrWhiteSpace(NambaDoctorContext.OrganisationId))
                    {
                        var serviceProviderProfile = serviceProvider.Profiles.Find(
                                    profile => profile.OrganisationId == NambaDoctorContext.OrganisationId);


                        if (serviceProviderProfile != null && serviceProviderProfile.ServiceProviderType != null)
                        {
                            ndLogger.LogEvent($"Setting designation: {serviceProviderProfile.ServiceProviderType}", SeverityLevel.Information);

                            NambaDoctorContext.Designation = serviceProviderProfile.ServiceProviderType;


                            //This is only to make reading log easier

                            ndLogger.LogEvent($"Service Provider First Name: {serviceProviderProfile.FirstName} ," +
                                $" LastName: {serviceProviderProfile.LastName}", SeverityLevel.Information);

                        }
                        else
                        {
                            ndLogger.LogEvent("Designation data not available", SeverityLevel.Error);

                            throw new InvalidDataException("Designation data not available");
                        }
                    }
                    else
                    {
                        ndLogger.LogEvent("SetDefaultOrganisation", SeverityLevel.Information);
                        await SetDefaultOrganisation();
                    }


                }
                else
                {
                    ndLogger.LogEvent("Setting User Not registered");
                    NambaDoctorContext.ndUserType = NDUserType.NotRegistered;
                    NambaDoctorContext.Designation = "NotRegistered";
                }


            }

            finally
            {
                timer.Stop();
                ndLogger.LogEvent($"SetUserInfo Execution time: {timer.ElapsedMilliseconds.ToString()}", SeverityLevel.Information);
            }

        }

        private async Task SetDefaultOrganisation()
        {
            string organisationId = await authService.GetDefaultOrganisationId();

            if (string.IsNullOrWhiteSpace(organisationId))
            {
                ndLogger.LogEvent("NotabletoSetDefaultOrganisation", SeverityLevel.Error);
                throw new InvalidDataException("Missing Organisation data");
            }
            else
            {
                NambaDoctorContext.OrganisationId = organisationId;
                ndLogger.LogEvent($"SetDefaultOrganisation: {NambaDoctorContext.OrganisationId.ToString()}", SeverityLevel.Information);

            }
        }

    }
}
