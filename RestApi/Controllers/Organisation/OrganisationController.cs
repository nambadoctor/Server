using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System;
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
        private IOrganisationService organisationService;

        public OrganisationController(NambaDoctorContext nambaDoctorContext, IOrganisationService organisationService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.organisationService = organisationService;
        }

        [HttpGet("{organisationId}")]
        [Authorize]
        public async Task<Organisation> GetOrganisation(string OrganisationId)
        {

            if (string.IsNullOrWhiteSpace(OrganisationId))
            {
                throw new ArgumentException("Organisation Id was null");
            }

            var organisations = await organisationService.GetOrganisationAsync(OrganisationId);

            return organisations;
        }
    }
}
