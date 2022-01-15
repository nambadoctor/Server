using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IReportService
    {
        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string ServiceRequestId);
        public Task SetReport(ProviderClientIncoming.ReportIncoming reportIncoming);
        public Task DeleteReport(string ServiceRequestId, string ReportId);
    }
}
