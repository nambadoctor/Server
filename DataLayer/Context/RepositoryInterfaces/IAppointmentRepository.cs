using DataModel.Mongo;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IAppointmentRepository : IRepository<ServiceProvider>
    {
        public Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId);
        public Task<List<Appointment>> GetAppointmentsByServiceProvider(string organisationId, List<string> serviceProviderIds);
        public Task AddAppointment(Appointment appointment);
        public Task CancelAppointment(Appointment appointment);
        public Task RescheduleAppointment(Appointment appointment);
        public Task EndAppointment(Appointment appointment);
    }
}
