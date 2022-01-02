using DataModel.Client.Provider;
using DataModel.Shared;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/serviceprovider")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private IServiceProviderService serviceProviderService;
        private ILogger logger;

        public ServiceProviderController(NambaDoctorContext nambaDoctorContext, IServiceProviderService serviceProviderService, ILogger<ServiceProviderController> logger)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.serviceProviderService = serviceProviderService;
            this.logger = logger;
        }

        [HttpGet("{ServiceProviderId}/organisation/{OrganisationId}")]
        public async Task<ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderController:GetServiceProviderAsync"))

            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("Starting null check");

                    if(string.IsNullOrWhiteSpace(ServiceProviderId) || !ObjectId.TryParse(ServiceProviderId, out var spid ))
                    {
                        throw new ArgumentNullException("ServiceProviderController:GetServiceProviderAsync ServiceProviderId is null");
                    }

                    if (string.IsNullOrWhiteSpace(OrganisationId) || !ObjectId.TryParse(OrganisationId, out var orgid))
                    {
                        throw new ArgumentNullException("ServiceProviderController:GetServiceProviderAsync OrganisationId is null");
                    }
                    NambaDoctorContext.AddTraceContext("OrganisationId", OrganisationId);
                    NambaDoctorContext.AddTraceContext("ServiceProviderId", ServiceProviderId);

                    logger.LogInformation("Start GetServiceProviderAsync");

                    var serviceProvider = await serviceProviderService.GetServiceProviderAsync(ServiceProviderId, OrganisationId);

                    logger.LogInformation("End GetServiceProviderAsync");

                    return serviceProvider;
                }
                finally
                {
                    logger.LogInformation("Finally GetServiceProviderAsync");

                }
            }

        }

        /// <summary>
        /// Usaully this will be the first method to be called to get Orgmemberships of a service provider
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ServiceProviderBasic> GetServiceProviderOrganisationMemeberships()
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderController:GetServiceProviderOrganisationMemeberships"))

            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("Start: GetServiceProviderOrganisationMemeberships");

                    var serviceProvider = await serviceProviderService.GetServiceProviderOrganisationMemeberships();

                    logger.LogInformation("SP Exists: Ctrl:GetServiceProviderOrganisationMemeberships");

                    return serviceProvider;
                }
                finally
                {
                    logger.LogInformation("End: Ctrl:GetServiceProviderOrganisationMemeberships");
                }
            }
        }
    }
}
