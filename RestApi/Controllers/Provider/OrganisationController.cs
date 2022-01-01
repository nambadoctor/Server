using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private NambaDoctorContext nambaDoctorContext;
        private IOrganisationService organisationService;
        private IAppointmentService appointmentService;
        private ICustomerService customerService;

        public OrganisationController(NambaDoctorContext nambaDoctorContext, IOrganisationService organisationService, IAppointmentService appointmentService, ICustomerService customerService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.organisationService = organisationService;
            this.appointmentService = appointmentService;
            this.customerService = customerService;
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

        [HttpGet("{organisationId}/appointments")]
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

        [HttpGet("{organisationId}/customers")]
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
