using DataModel.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IServiceProviderRepository : IRepository<ServiceProvider>
    {
        public Task<List<Appointment>> GetAppointmentsByServiceProvider(string organisationId, List<string> serviceProviderIds);
    }
}
