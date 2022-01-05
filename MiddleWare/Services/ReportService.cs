using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class ReportService : IReportService
    {
        public Task<string> DeleteReport(string CustomerId, string AppointmentId, string ReportId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string AppointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<ProviderClientOutgoing.ReportOutgoing> SetReport(string CustomerId, ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            throw new NotImplementedException();
        }
    }
}
