using DataModel.Shared;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using MiddleWare.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApi.Middlewares
{
    public class UserinfoMiddleware
    {
        private readonly RequestDelegate _next;
        private INDLogger ndLogger;
        private IAuthService authService;
        public UserinfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService, INDLogger ndLogger)
        {
            this.authService = authService;
            this.ndLogger = ndLogger;

            foreach (Claim claim in context.User.Claims)
            {
                if(claim.Type == "phone_number")
                {
                    NambaDoctorContext.PhoneNumber = claim.Value;
                }
            }


            _ = SetUserInfo();
            await _next(context);
        }

        private async Task SetUserInfo()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {

                var serviceProvider =  authService.GetServiceProviderFromRegisteredPhoneNumber().GetAwaiter().GetResult();

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
