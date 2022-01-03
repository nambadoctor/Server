using Client = DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Client.Appointment> GetAppointment(string serviceProviderId, string appointmentId);
        public Task<List<Client.Appointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds);
        public Task<Client.Appointment> SetAppointment(Client.Appointment appointment);
    }
}
