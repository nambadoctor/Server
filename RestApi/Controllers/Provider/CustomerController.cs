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

        [HttpGet("{Customerid}/{Organisationid}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string CustomerId, string OrganisationId)
        {

            if (string.IsNullOrWhiteSpace(CustomerId))
            {
                throw new ArgumentException("Customer Id was null");
            }

            var customerProfile = await customerService.GetCustomerProfile(CustomerId, OrganisationId);

            return customerProfile;
        }

        [HttpGet("{PhoneNumber}/{Organisationid}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string PhoneNumber, string OrganisationId)
        {

            var customerProfile = await customerService.GetCustomerProfileFromPhoneNumber(PhoneNumber, OrganisationId);

            return customerProfile;
        }

        [HttpPut()]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> SetCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            var customerProfileToReturn = await customerService.SetCustomerProfile(customerProfile);

            return customerProfileToReturn;
        }

        [HttpPut("appointment")]
        [Authorize]
        public async Task<ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing> SetCustomerProfile([FromBody] ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointment)
        {
            var customerProfileToReturn = await customerService.SetCustomerProfileWithAppointment(customerProfileWithAppointment);

            return customerProfileToReturn;
        }

        [HttpGet("{Customerid}/report/{AppointmentId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string AppointmentId)
        {

            var customerProfile = await reportService.GetAppointmentReports(CustomerId, AppointmentId);

            return customerProfile;
        }

        [HttpDelete("{Customerid}/report/{AppointmentId}/{ReportId}")]
        [Authorize]
        public async Task<string> DeleteReport(string CustomerId, string AppointmentId, string ReportId)
        {

            var reportId = await reportService.DeleteReport(CustomerId, AppointmentId, ReportId);

            return reportId;
        }

        [HttpPut("{Customerid}/report")]
        [Authorize]
        public async Task<ProviderClientOutgoing.ReportOutgoing> SetReport(string CustomerId, [FromBody] ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            var report = await reportService.SetReport(CustomerId, reportIncoming);

            return report;
        }

        [HttpGet("{Customerid}/prescription/{AppointmentId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptionDocuments(string CustomerId, string AppointmentId)
        {

            var prescriptionDocuments = await prescriptionService.GetAppointmentPrescriptions(CustomerId, AppointmentId);

            return prescriptionDocuments;
        }

        [HttpDelete("{Customerid}/prescription/{AppointmentId}/{PrescriptionDocumentId}")]
        [Authorize]
        public async Task<string> DeletePrescriptionDocument(string CustomerId, string AppointmentId, string PrescriptionDocumentId)
        {

            var reportId = await prescriptionService.DeletePrescriptionDocument(CustomerId, AppointmentId, PrescriptionDocumentId);

            return reportId;
        }

        [HttpPut("{Customerid}/prescription")]
        [Authorize]
        public async Task<ProviderClientOutgoing.PrescriptionDocumentOutgoing> SetPrescriptionDocument(string CustomerId, [FromBody] ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            var prescriptionDocument = await prescriptionService.SetPrescriptionDocument(CustomerId, prescriptionDocumentIncoming);

            return prescriptionDocument;
        }

    }
}
