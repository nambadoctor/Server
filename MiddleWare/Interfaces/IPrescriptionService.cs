using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IPrescriptionService
    {
        public Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string CustomerId, string AppointmentId);
        public Task<ProviderClientOutgoing.PrescriptionDocumentOutgoing> SetPrescriptionDocument(string CustomerId, ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming);
        public Task<string> DeletePrescriptionDocument(string CustomerId, string AppointmentId, string PrescriptionDocumentId);
    }
}
