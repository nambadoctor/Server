using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [Authorize]
        public async Task<ProviderClientOutgoing.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            var serviceProvider = await serviceProviderService.GetServiceProviderAsync(ServiceProviderId, OrganisationId);

            return serviceProvider;

        }
        
        [HttpGet("organisation/{OrganisationId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ServiceProvider>> GetServiceProvidersAsync(string OrganisationId)
        {
            var serviceProviders = await serviceProviderService.GetServiceProvidersAsync(OrganisationId);

            return serviceProviders;

        }

        /// <summary>
        /// Usaully this will be the first method to be called to get Orgmemberships of a service provider
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemberships()
        {
            var serviceProvider = await serviceProviderService.GetServiceProviderOrganisationMemberships();
            return serviceProvider;
        }
    }
}
