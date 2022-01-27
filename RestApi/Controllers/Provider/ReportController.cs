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
    [Route("api/provider/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        private IReportService reportService;

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }


        [HttpGet("{ServiceRequestId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string ServiceRequestId)
        {

            var reports = await reportService.GetAppointmentReports(ServiceRequestId);

            return reports;
        }

        [HttpGet("all/{OrganisationId}/{CustomerId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetReports(string OrganisationId, string CustomerId)
        {

            var reports = await reportService.GetAllReports(OrganisationId, CustomerId);

            return reports;
        }

        [HttpDelete("{ServiceRequestId}/{ReportId}")]
        [Authorize]
        public async Task DeleteReport(string ServiceRequestId, string ReportId)
        {
            await reportService.DeleteReport(ServiceRequestId, ReportId);
        }

        [HttpPost("")]
        [Authorize]
        public async Task SetReport([FromBody] ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            await reportService.SetReport(reportIncoming);
        }

        [HttpPost("Stray/{ServiceProviderId}/{CustomerId}")]
        [Authorize]
        public async Task SetStrayReport([FromBody] ProviderClientIncoming.ReportIncoming reportIncoming, string ServiceProviderId, string CustomerId)
        {
            await reportService.SetStrayReport(reportIncoming, ServiceProviderId, CustomerId);
        }
    }
}
