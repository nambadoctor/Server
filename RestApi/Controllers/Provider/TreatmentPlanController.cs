using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/treatmentplan")]
    [ApiController]
    public class TreatmentPlanController : ControllerBase
    {
        private readonly ITreatmentPlanService treatmentPlanService;
        public TreatmentPlanController(ITreatmentPlanService treatmentPlanService)
        {
            this.treatmentPlanService = treatmentPlanService;
        }

        [HttpGet("all/{OrganisationId}/{ServiceproviderId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetAllTreatmentPlans(string OrganisationId, string ServiceproviderId)
        {

            var treatments = await treatmentPlanService.GetAllTreatmentPlans(OrganisationId, ServiceproviderId);

            return treatments;
        }

        [HttpGet("customer/{OrganisationId}/{CustomerId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetCustomerTreatmentPlans(string OrganisationId, string CustomerId)
        {

            var treatments = await treatmentPlanService.GetCustomerTreatmentPlans(OrganisationId, CustomerId);

            return treatments;
        }

        [HttpPut("")]
        [Authorize]
        public async Task UpdateTreatmentPlan([FromBody] ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming)
        {

            await treatmentPlanService.UpdateTreatmentPlan(treatmentPlanIncoming);

        }

        [HttpPost("treatment/{TreatmentPlanId}")]
        [Authorize]
        public async Task AddTreatment(string TreatmentPlanId, [FromBody] ProviderClientIncoming.TreatmentIncoming treatmentIncoming)
        {

            await treatmentPlanService.AddTreatment(TreatmentPlanId, treatmentIncoming);

        }

        [HttpPut("treatment/{TreatmentPlanId}")]
        [Authorize]
        public async Task UpdateTreatment(string TreatmentPlanId, [FromBody] ProviderClientIncoming.TreatmentIncoming treatmentIncoming)
        {

            await treatmentPlanService.UpdateTreatment(TreatmentPlanId, treatmentIncoming);

        }

        [HttpDelete("treatment/{TreatmentPlanId}/{TreatmentId}")]
        [Authorize]
        public async Task DeleteTreatment(string TreatmentPlanId, string TreatmentId)
        {

            await treatmentPlanService.DeleteTreatment(TreatmentPlanId, TreatmentId);

        }

    }
}
