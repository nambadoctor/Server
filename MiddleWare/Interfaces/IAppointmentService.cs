using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IAppointmentService
    {
        public Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string serviceProviderId, string appointmentId);
        public Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds);
        public Task<ProviderClientOutgoing.OutgoingAppointment> SetAppointment(ProviderClientIncoming.AppointmentIncoming appointment);
    }
}
