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
        private INDLogger ndLogger;
        private IServiceProviderService serviceProviderService;

        public ServiceProviderController(NambaDoctorContext nambaDoctorContext, IServiceProviderService serviceProviderService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.serviceProviderService = serviceProviderService;
            ndLogger = this.nambaDoctorContext._NDLogger;
        }

        [HttpGet]
        public async Task<ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            ndLogger.LogEvent("Start GetServiceProviderAsync");

            var serviceProvider = await serviceProviderService.GetServiceProviderAsync(ServiceProviderId, OrganisationId);

            ndLogger.LogEvent("End GetServiceProviderAsync");
            return serviceProvider;
        }

        [HttpGet]
        public async Task<ServiceProvider> GetServiceProviderWithOrganisationsAsync()
        {
            ndLogger.LogEvent("Start GetServiceProviderAsync");

            var serviceProvider = await serviceProviderService.GetServiceProviderOrganisationsAsync();

            ndLogger.LogEvent("End GetServiceProviderAsync");
            return serviceProvider;
        }
    }
}
