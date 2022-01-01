using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System;
using System.Threading.Tasks;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private ICustomerService customerService;

        public CustomerController(NambaDoctorContext nambaDoctorContext, ICustomerService customerService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.customerService = customerService;
        }

        [HttpGet("{Customerid}/{Organisationid}")]
        [Authorize]
        public async Task<CustomerProfile> GetOrganisation(string CustomerId, string OrganisationId)
        {

            if (string.IsNullOrWhiteSpace(CustomerId))
            {
                throw new ArgumentException("Customer Id was null");
            }

            var customerProfile = await customerService.GetCustomer(CustomerId, OrganisationId);

            return customerProfile;
        }
    }
}
