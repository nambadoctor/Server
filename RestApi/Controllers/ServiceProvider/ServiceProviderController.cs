using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NambaDoctorWebApi.Controllers.Providers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private IServiceProviderService serviceProviderService;

        public ServiceProviderController(NambaDoctorContext nambaDoctorContext, IServiceProviderService serviceProviderService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.serviceProviderService = serviceProviderService;
        }

        [HttpGet("{serviceProviderId}/{organisationId}")]
        public async Task<ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {

            var serviceProvider = await serviceProviderService.GetServiceProviderAsync(ServiceProviderId, OrganisationId);

            return serviceProvider;
        }

        [HttpGet]
        public async Task<ServiceProviderBasic> GetServiceProviderWithOrganisationsAsync()
        {

            var serviceProvider = await serviceProviderService.GetServiceProviderOrganisationsAsync();

            return serviceProvider;
        }
    }
}
