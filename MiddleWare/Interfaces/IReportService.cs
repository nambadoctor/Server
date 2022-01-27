using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IReportService
    {
        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string ServiceRequestId);
        public Task SetReport(ProviderClientIncoming.ReportIncoming reportIncoming);
        public Task SetStrayReport(ProviderClientIncoming.ReportIncoming reportIncoming, string ServiceProviderId, string CustomerId);
        public Task DeleteReport(string ServiceRequestId, string ReportId);
        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAllReports(string organisationId, string customerId);
    }
}