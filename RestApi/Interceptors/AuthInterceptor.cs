using DataLayer;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ServerDataModels.Local;
using System;
using System.Threading.Tasks;

namespace NambaDoctorWebApi.Interceptors
{
    public class AuthInterceptor
    {
        private readonly RequestDelegate requestDelegate;
        private INDLogger ndLogger;
        private IMongoDbDataLayer dataLayer;

        public AuthInterceptor(IMongoDbDataLayer dataLayer, INDLogger ndLogger, RequestDelegate requestDelegate)
        {
            this.dataLayer = dataLayer;
            this.ndLogger = ndLogger;
            this.requestDelegate = requestDelegate;
        }

        //public AuthInterceptor(RequestDelegate requestDelegate)
        //{
        //    this.requestDelegate = requestDelegate;
        //}

        [Authorize]
        public async Task Invoke(HttpContext context)
        {
            // Skip token validation for Ping API ONLY
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

                        InitializeNDContext(decodedAuthToken);

                        await InitializeUserIdFromDb();

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
