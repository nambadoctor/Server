using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IReportService
    {
        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string AppointmentId);
        public Task<ProviderClientOutgoing.ReportOutgoing> SetReport(string CustomerId, ProviderClientIncoming.ReportIncoming reportIncoming);
        public Task<string> DeleteReport(string CustomerId, string AppointmentId, string ReportId);
    }
}
