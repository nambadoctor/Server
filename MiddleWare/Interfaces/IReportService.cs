using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IReportService
    {
        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string ServiceRequestId);
        public Task SetReport(string CustomerId, ProviderClientIncoming.ReportIncoming reportIncoming);
        public Task DeleteReport(string CustomerId, string ServiceRequestId, string ReportId);
    }
}
