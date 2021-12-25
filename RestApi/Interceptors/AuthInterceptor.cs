using DataLayer;
using DataModel.Shared;
using FirebaseAdmin.Auth;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private IMongoDbDataLayer dataLayer; //[ToDo- Rest layer should never have reference to Dataaccess

        public AuthInterceptor(IMongoDbDataLayer dataLayer, INDLogger ndLogger, RequestDelegate requestDelegate)
        {
            this.dataLayer = dataLayer;
            this.ndLogger = ndLogger;
            this.requestDelegate = requestDelegate;
        }
        private async Task ValidateTokenAndSetContext(HttpContext context)
        {
            var authCreds = context.Request.Headers.GetValue(Constants.Auth.Header);
            if (authCreds == null)
            {
                ndLogger.LogEvent("Auth header does not exist", SeverityLevel.Error);
                throw new UnauthorizedAccessException("Unauthorized access not allowed");
            }

            ndLogger.LogEvent($"AuthCreds: {authCreds}", SeverityLevel.Information);

            var authTokenParts = authCreds.Split(Constants.Auth.Spliter);
            var authToken = string.Empty;

            if (authTokenParts.Length > 1) // [TBD] - What is the correct way here?
            {
                authToken = authTokenParts[1];
            }
            else
            {
                authToken = authTokenParts[0];
            }

            ndLogger.LogEvent($"AuthToken: {authToken}", SeverityLevel.Information);

            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(authToken);
            // [TBD] Do we need to log all keys for any reason?

            NambaDoctorContext.FirebaseUserId = decoded.Uid;
            ndLogger.LogEvent($"FirebaseUserId: {decoded.Uid}", SeverityLevel.Information);

            decoded.Claims.TryGetValue(Constants.Auth.PhoneNumber, out object phoneNumber);

            if (phoneNumber != null)
            {
                NambaDoctorContext.PhoneNumber = phoneNumber.ToString();
                ndLogger.LogEvent($"Authenticaed for Phone Number: {phoneNumber.ToString()}", SeverityLevel.Information);

                //[TBD] - Check for blocked users
                //[TBD] - Set TestUserType

            }
            else if (Constants.Auth.anonymousAllowList.Contains(context.Method))
            {
                ndLogger.LogEvent($"Anonymous auth for valid api: {context.Method}", SeverityLevel.Information);

                /*[TBD: We are getting into call flows with out phone number set. 
                 * Will it have impact in database calls any other calls that has Phone Number depdency
                */
            }
            else
            {
                ndLogger.LogEvent($"Anonymous auth for invalid api: {context.Method}", SeverityLevel.Error);
                throw new UnauthorizedAccessException("Unauthorized access not allowed");
            }

        }
        private void SetContextValues(HttpContext context)
        {
            NambaDoctorContext.ContextValues = StoreContextValues(context);
        }
        private async Task SetUserInfo()
        {
            Stopwatch timer = new Stopwatch();

            try
            {
                var customer = await dataLayer.GetCustomerFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

                if (customer != null)
                {
                    ndLogger.LogEvent($"Setting Customer Id :{customer.CustomerId.ToString()}");

                    NambaDoctorContext.NDUserId = customer.CustomerId.ToString();
                    NambaDoctorContext.ndUserType = NDUserType.Customer;
                    NambaDoctorContext.Designation = "Customer";

                    return;
                }

                var serviceProvider = await dataLayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

                if (serviceProvider != null)
                {
                    ndLogger.LogEvent($"Setting serviceProvider Id :{serviceProvider.ServiceProviderId.ToString()}");

                    NambaDoctorContext.NDUserId = serviceProvider.ServiceProviderId.ToString();
                    NambaDoctorContext.ndUserType = NDUserType.ServiceProvider;
                    NambaDoctorContext.Designation = serviceProvider.ServiceProviderType;

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
        private async Task SetIfTestUser()
        {
            bool isTestUser = await UtilityFunctions.IsTestPhoneNumber(NambaDoctorContext.PhoneNumber, _dataLayer);
            ndLogger.LogEvent($"Request from test user:{isTestUser}");
            NambaDoctorContext.IsTestUser = isTestUser;
        }

        private Dictionary<string, string> StoreContextValues(HttpContext context)
        {
            var traceDictionary = new Dictionary<string, string>();

            if(context.Request.Headers.TryGetValue("userid", out var userId))
            {
                traceDictionary.Add("UserId", userId);
            }

            if(context.Request.Headers.TryGetValue("usertype", out var userType))
            {
                traceDictionary.Add("UserType", userType);

            }

            if(context.Request.Headers.TryGetValue("eventdatetime", out var eventDateTime))
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



            if (organisationId != null)
            {
                traceDictionary.Add("OrganisationId", organisationId);
                NambaDoctorContext.OrganisationId = organisationId;
            }
            if (eventDateTime != null)
            if (appointmentId != null)
                traceDictionary.Add("AppointmentId", appointmentId);
            if (sessionId != null)
                traceDictionary.Add("SessionId", sessionId);
            if (correlationId != null)
                traceDictionary.Add("CorrelationId", correlationId);
            if (appVersion != null)
                traceDictionary.Add("AppVersion", appVersion);
            if (eventMessage != null)
                traceDictionary.Add("EventMessage", eventMessage);
            if (logLevel != null)
                traceDictionary.Add("LogLevel", logLevel);
            if (isProduction != null)
                traceDictionary.Add("IsProduction", isProduction);
            if (deviceInfo != null)
                traceDictionary.Add("DeviceInfo", deviceInfo);
            if (eventType != null)
                traceDictionary.Add("EventType", eventType);
            if (notificationMessageId != null)
                traceDictionary.Add("NotificationMessageId", notificationMessageId);

            return traceDictionary;
        }
       
        [Authorize]
        public async Task Invoke(HttpContext context)
        { 
            Stopwatch timer = new Stopwatch();

            try
            {
                SetContextValues(context);

                ndLogger.LogEvent("Context values are set", SeverityLevel.Information);

                ndLogger.LogEvent("ValidateTokenAndSetContext", SeverityLevel.Information);
                await ValidateTokenAndSetContext(context);

                ndLogger.LogEvent("SetUserInfo", SeverityLevel.Information);
                await SetUserInfo();

                ndLogger.LogEvent("SetUserIsTest", SeverityLevel.Information);
                await SetIfTestUser();

                ndLogger.LogEvent("Continuation UnaryServerHandler", SeverityLevel.Information);
            }
            catch (Exception ex)
            {
                ndLogger.LogEvent($"Exception in UnaryServerHandler: {ex.ToString()}", SeverityLevel.Error);
                //[ToDo]throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
            finally
            {
                timer.Stop();
                ndLogger.LogEvent($"UnaryServerHandler Execution time: {timer.ElapsedMilliseconds.ToString()}", SeverityLevel.Information);

            }
        }


        private void InitializeNDContext(FirebaseToken firebaseToken)
        {
            foreach (var claim in firebaseToken.Claims)
            {
                if (claim.Key.Equals("phone_number", StringComparison.OrdinalIgnoreCase))
                {
                    NambaDoctorContext.PhoneNumber = claim.Value.ToString();
                    break;
                }
            }
        }

        private async Task InitializeUserIdFromDb()
        {
            ndLogger.LogEvent("Getting user info from Db");

            var userInfo = await dataLayer.GetUserTypeFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);
            if (userInfo == null || userInfo == "NotRegistered")
            {
                ndLogger.LogEvent("User does not exist in Db");
            }
            else
            {
                string[] userTypeAndId = userInfo.Split(",");
                string userType = userTypeAndId[0];
                string userId = userTypeAndId[1];
                NambaDoctorContext.NDUserId = userId;

                if (userType.Contains("ServiceProvider"))
                {
                    NambaDoctorContext.ndUserType = NDUserType.ServiceProvider;
                }
                else if (userType.Contains("Customer"))
                {
                    NambaDoctorContext.ndUserType = NDUserType.Customer;
                }
            }
        }
    }
}
