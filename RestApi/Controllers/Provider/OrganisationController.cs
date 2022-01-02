﻿using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiddleWare.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/organisation")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private IOrganisationService organisationService;
        private IAppointmentService appointmentService;
        private ICustomerService customerService;
        private ILogger logger;
        
        public OrganisationController(IOrganisationService organisationService, 
                                        IAppointmentService appointmentService, 
                                        ICustomerService customerService,
                                        ILogger<OrganisationController> logger)
        {
            this.organisationService = organisationService;
            this.appointmentService = appointmentService;
            this.customerService = customerService;
            this.logger = logger;
        }

        [HttpGet("{OrganisationId}")]
        [Authorize]
        public async Task<Organisation> GetOrganisation(string OrganisationId)
        {
            using (logger.BeginScope("Method: {Method}", "OrganisationController:GetOrganisation"))

            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("Start GetOrganisation");

                    var organisations = await organisationService.GetOrganisationAsync(OrganisationId);

                    return organisations;
                }
                finally
                {
                    logger.LogInformation("End GetOrganisation");
                }
            }
        }

        [HttpGet("{OrganisationId}/appointments")]
        [Authorize]
        public async Task<List<Appointment>> GetOrganisationAppointments(string OrganisationId, [FromQuery] List<string> ServiceProviderIds)
        {
            if (string.IsNullOrWhiteSpace(OrganisationId))
            {
                throw new ArgumentException("Organisation Id was null");
            }

            var appointments = await appointmentService.GetAppointments(OrganisationId, ServiceProviderIds);

            return appointments;
        }

        [HttpGet("{OrganisationId}/customers")]
        [Authorize]
        public async Task<List<CustomerProfile>> GetOrganisationCustomers(string OrganisationId, [FromQuery] List<string> ServiceProviderIds)
        {
            if (string.IsNullOrWhiteSpace(OrganisationId))
            {
                throw new ArgumentException("Organisation Id was null");
            }

            var customers = await customerService.GetCustomers(OrganisationId, ServiceProviderIds);

            return customers;
        }
    }
}
