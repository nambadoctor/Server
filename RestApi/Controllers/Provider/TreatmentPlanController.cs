﻿using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
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

        [HttpGet("{OrganisationId}/{ServiceproviderId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetAllTreatmentPlans(string OrganisationId, string ServiceproviderId, [FromQuery]string CustomerId)
        {

            var treatmentPlans = await treatmentPlanService.GetTreatmentPlans(OrganisationId, ServiceproviderId, CustomerId);

            return treatmentPlans;
        }
        
        [HttpGet("treatments/{OrganisationId}/{ServiceproviderId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.TreatmentOutgoing>> GetTreatments(string OrganisationId, string ServiceproviderId, [FromQuery] string CustomerId, [FromQuery] bool IsUpcoming)
        {

            var treatments = await treatmentPlanService.GetTreatments(OrganisationId, ServiceproviderId, CustomerId, IsUpcoming);

            return treatments;

        }


        [HttpPost("")]
        [Authorize]
        public async Task AddTreatmentPlan([FromBody] ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming)
        {

            await treatmentPlanService.AddTreatmentPlan(treatmentPlanIncoming);

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
