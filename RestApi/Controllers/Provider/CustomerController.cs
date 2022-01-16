using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private ICustomerService customerService;
        private IAppointmentService appointmentService;
        public CustomerController(NambaDoctorContext nambaDoctorContext, ICustomerService customerService, IAppointmentService appointmentService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.customerService = customerService;
        }

        [HttpGet("{CustomerId}/{OrganisationId}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string CustomerId, string OrganisationId)
        {

            var customerProfile = await customerService.GetCustomerProfile(CustomerId, OrganisationId);

            return customerProfile;
        }

        [HttpGet("phonenumber/{PhoneNumber}/{OrganisationId}")] //Here phone number cannot contain + as its not allowed in .Net query string
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string PhoneNumber, string OrganisationId)
        {

            var customerProfile = await customerService.GetCustomerProfileFromPhoneNumber(PhoneNumber, OrganisationId);

            return customerProfile;
        }

        [HttpPost("")]
        [Authorize]
        public async Task AddCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            await customerService.AddCustomerProfile(customerProfile);
        }

        [HttpPut("")]
        [Authorize]
        public async Task UpdateCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            await customerService.UpdateCustomerProfile(customerProfile);
        }

        [HttpPost("appointment")]
        [Authorize]
        public async Task SetCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointment)
        {
            await customerService.AddCustomerProfile(customerProfileWithAppointment.CustomerProfileIncoming);
            await appointmentService.AddAppointment(customerProfileWithAppointment.AppointmentIncoming);
        }

    }
}
