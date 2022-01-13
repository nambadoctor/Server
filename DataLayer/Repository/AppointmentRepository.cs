using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class AppointmentRepository : BaseRepository<ServiceProvider>, IAppointmentRepository
    {
        public AppointmentRepository(IMongoContext context) : base(context)
        {
        }
        public Task AddAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Appointment>> GetAppointmentsByServiceProvider(string organisationId, List<string> serviceProviderIds)
        {

            var serviceProviderIdList = new List<ObjectId>();
            foreach (var spId in serviceProviderIds)
            {
                serviceProviderIdList.Add(new ObjectId(spId));
            }

            var organisationAppointmentFilter = Builders<ServiceProvider>.Filter.ElemMatch(
                sp => sp.Appointments,
                appointment => appointment.OrganisationId == organisationId
                );

            var serviceProviderFilter = Builders<ServiceProvider>.Filter.In(
                sp => sp.ServiceProviderId,
                serviceProviderIdList
                );

            FilterDefinition<ServiceProvider> combinedFilter;
            if (serviceProviderIds.Count == 0)
            {
                combinedFilter = organisationAppointmentFilter;
            }
            else
            {
                combinedFilter = organisationAppointmentFilter & serviceProviderFilter;
            }

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Appointments.Where(appointment => appointment.OrganisationId == organisationId)
                );

            var result = await this.GetListByFilterAndProject(combinedFilter, project);

            return result.ToList();
        }

        public Task UpdateAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}
