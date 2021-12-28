﻿using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NambaDoctorWebApi.Controllers.Providers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger ndLogger;
        private IOrganisationService organisationService;

        public OrganisationController(NambaDoctorContext nambaDoctorContext, IOrganisationService organisationService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.organisationService = organisationService;
            ndLogger = this.nambaDoctorContext._NDLogger;
        }

        [HttpGet("{id}")]
        public async Task<Organisation> GetOrganisation(string OrganisationId)
        {
            ndLogger.LogEvent("Start GetOrganisations");

            var organisations = await organisationService.GetOrganisationAsync(OrganisationId);

            ndLogger.LogEvent("End GetOrganisations");
            return organisations;
        }
    }
}
