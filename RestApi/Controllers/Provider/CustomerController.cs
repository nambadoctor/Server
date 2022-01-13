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
        private IReportService reportService;
        private IPrescriptionService prescriptionService;

        public CustomerController(NambaDoctorContext nambaDoctorContext, ICustomerService customerService, IReportService reportService, IPrescriptionService prescriptionService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.customerService = customerService;
            this.reportService = reportService;
            this.prescriptionService = prescriptionService;
        }

        [HttpGet("{CustomerId}/{OrganisationId}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string CustomerId, string OrganisationId)
        {

            var customerProfile = await customerService.GetCustomerProfile(CustomerId, OrganisationId);

            return customerProfile;
        }

        [HttpGet("CheckByPhoneNumber/{PhoneNumber}/{OrganisationId}")] //Here phone number cannot contain + as its not allowed in .Net query string
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string PhoneNumber, string OrganisationId)
        {

            var customerProfile = await customerService.GetCustomerProfileFromPhoneNumber(PhoneNumber, OrganisationId);

            return customerProfile;
        }

        [HttpPut()]
        [Authorize]
        public async Task SetCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            await customerService.SetCustomerProfile(customerProfile);
        }

        [HttpPut("appointment")]
        [Authorize]
        public async Task SetCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointment)
        {
            await customerService.SetCustomerProfileWithAppointment(customerProfileWithAppointment);
        }

        [HttpGet("{CustomerId}/report/{AppointmentId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string AppointmentId)
        {

            var customerProfile = await reportService.GetAppointmentReports(CustomerId, AppointmentId);

            return customerProfile;
        }

        [HttpDelete("{CustomerId}/report/{AppointmentId}/{ReportId}")]
        [Authorize]
        public async Task DeleteReport(string CustomerId, string AppointmentId, string ReportId)
        {
            await reportService.DeleteReport(CustomerId, AppointmentId, ReportId);
        }

        [HttpPut("{CustomerId}/report")]
        [Authorize]
        public async Task SetReport(string CustomerId, [FromBody] ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            await reportService.SetReport(CustomerId, reportIncoming);
        }

        [HttpGet("{CustomerId}/prescription/{AppointmentId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptionDocuments(string CustomerId, string AppointmentId)
        {

            var prescriptionDocuments = await prescriptionService.GetAppointmentPrescriptions(CustomerId, AppointmentId);

            return prescriptionDocuments;
        }

        [HttpDelete("{CustomerId}/prescription/{AppointmentId}/{PrescriptionDocumentId}")]
        [Authorize]
        public async Task DeletePrescriptionDocument(string CustomerId, string AppointmentId, string PrescriptionDocumentId)
        {
            await prescriptionService.DeletePrescriptionDocument(CustomerId, AppointmentId, PrescriptionDocumentId);
        }

        [HttpPut("{CustomerId}/prescription")]
        [Authorize]
        public async Task SetPrescriptionDocument(string CustomerId, [FromBody] ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            await prescriptionService.SetPrescriptionDocument(CustomerId, prescriptionDocumentIncoming);
        }

    }
}
