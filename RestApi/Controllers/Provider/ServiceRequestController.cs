using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/servicerequest")]
    [ApiController]
    public class ServiceRequestController
    {
        private NambaDoctorContext nambaDoctorContext;
        private IPrescriptionService prescriptionService;
        private IReportService reportService;
        private IServiceRequestService serviceRequestService;

        public ServiceRequestController(NambaDoctorContext nambaDoctorContext, IPrescriptionService prescriptionService, IReportService reportService, IServiceRequestService serviceRequestService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.prescriptionService = prescriptionService;
            this.reportService = reportService;
            this.serviceRequestService = serviceRequestService;
        }

        [HttpGet("{CustomerId}/report/{ServiceRequestId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string ServiceRequestId)
        {

            var customerProfile = await reportService.GetAppointmentReports(CustomerId, ServiceRequestId);

            return customerProfile;
        }

        [HttpDelete("{CustomerId}/report/{ServiceRequestId}/{ReportId}")]
        [Authorize]
        public async Task DeleteReport(string CustomerId, string ServiceRequestId, string ReportId)
        {
            await reportService.DeleteReport(CustomerId, ServiceRequestId, ReportId);
        }

        [HttpPost("{CustomerId}/report")]
        [Authorize]
        public async Task SetReport(string CustomerId, [FromBody] ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            await reportService.SetReport(CustomerId, reportIncoming);
        }

        [HttpGet("{CustomerId}/prescription/{ServiceRequestId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptionDocuments(string CustomerId, string ServiceRequestId)
        {

            var prescriptionDocuments = await prescriptionService.GetAppointmentPrescriptions(CustomerId, ServiceRequestId);

            return prescriptionDocuments;
        }

        [HttpDelete("{CustomerId}/prescription/{ServiceRequestId}/{PrescriptionDocumentId}")]
        [Authorize]
        public async Task DeletePrescriptionDocument(string CustomerId, string ServiceRequestId, string PrescriptionDocumentId)
        {
            await prescriptionService.DeletePrescriptionDocument(CustomerId, ServiceRequestId, PrescriptionDocumentId);
        }

        [HttpPost("{CustomerId}/prescription")]
        [Authorize]
        public async Task SetPrescriptionDocument(string CustomerId, [FromBody] ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            await prescriptionService.SetPrescriptionDocument(CustomerId, prescriptionDocumentIncoming);
        }
    }
}
