using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
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
        public async Task<ProviderClientOutgoing.Organisation> GetOrganisation(string OrganisationId)
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
        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetOrganisationAppointments(string OrganisationId, [FromQuery] List<string> ServiceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "OrganisationController:GetOrganisation"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("Start GetOrganisationAppointments");
                    var appointments = await appointmentService.GetAppointments(OrganisationId, ServiceProviderIds);
                    return appointments;
                }
                finally
                {
                    logger.LogInformation("End GetOrganisationAppointments");
                }
            }

        }

        [HttpGet("{OrganisationId}/customers")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetOrganisationCustomers(string OrganisationId, [FromQuery] List<string> ServiceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "OrganisationController:GetOrganisation"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("Start GetOrganisationCustomers");

                    var customers = await customerService.GetCustomerProfiles(OrganisationId, ServiceProviderIds);

                    return customers;
                }
                finally
                {
                    logger.LogInformation("End GetOrganisationCustomers");
                }
            }
        }

    }
}
