using Client = DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Client.AppointmentData> GetAppointment(string serviceProviderId, string appointmentId);
        public Task<List<Client.AppointmentData>> GetAppointments(string organsiationId, List<string> serviceProviderIds);
    }
}
