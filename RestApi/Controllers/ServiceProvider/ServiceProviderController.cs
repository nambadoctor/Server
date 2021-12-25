using Microsoft.AspNetCore.Mvc;
using NambaMiddleWare.Interfaces;
using DataModel.Shared;
using DataModel.Client.Provider;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NambaDoctorWebApi.Controllers
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
        public async Task<ServiceProvider> GetServiceProviderAsync()
        {
            // When call comes with No Service providerId and OrgId assume default organisation and return profile based on that
            ndLogger.LogEvent("Start GetServiceProviderAsync");
            var serviceProvider = await serviceProviderService.GetServiceProviderAsync();

            ndLogger.LogEvent("End GetServiceProviderAsync");
            return serviceProvider;
        }
    }
}
