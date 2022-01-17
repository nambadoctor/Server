using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/vitals")]
    [ApiController]
    public class VitalsController : ControllerBase
    {
        private IServiceRequestService serviceRequestService;

        public VitalsController(IServiceRequestService serviceRequestService)
        {
            this.serviceRequestService = serviceRequestService;
        }

        [HttpGet("{ServiceRequestId}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.VitalsOutgoing> GetVitals(string ServiceRequestId)
        {

            var vitals = await serviceRequestService.GetVitals(ServiceRequestId);

            return vitals;
        }

        [HttpPut("{ServiceRequestId}")]
        public async Task UpdateVitals([FromBody] ProviderClientIncoming.VitalsIncoming vitalsIncoming)
        {
            await serviceRequestService.UpdateVitals(vitalsIncoming);
        }
    }
}
