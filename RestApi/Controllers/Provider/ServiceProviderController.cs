using DataModel.Client.Provider;
using DataModel.Shared;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiddleWare.Interfaces;
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

        [HttpGet("{serviceProviderId}/organisation/{organisationId}")]
        public async Task<ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {

            var serviceProvider = await serviceProviderService.GetServiceProviderAsync(ServiceProviderId, OrganisationId);

            return serviceProvider;
        }

        /// <summary>
        /// Usaully this will be the first method to be called to get Orgmemberships of a service provider
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ServiceProviderBasic> GetServiceProviderOrganisationMemeberships()
        {

            try
            {
                logger.LogInformation("Request Start: GetServiceProviderOrganisationMemeberships" );

                var serviceProvider = await serviceProviderService.GetServiceProviderOrganisationMemeberships();

                return serviceProvider;
            }
            finally
            { 
                logger.LogInformation("Request End: GetServiceProviderOrganisationMemeberships");
            }

        }
    }
}
