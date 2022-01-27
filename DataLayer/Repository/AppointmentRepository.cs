using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class AppointmentRepository : BaseRepository<ServiceProvider>, IAppointmentRepository
    {
        public AppointmentRepository(IMongoContext context) : base(context)
        {
        }
        public async Task AddAppointment(Appointment appointment)
        {

            var filter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));

            var update = Builders<ServiceProvider>.Update.AddToSet(sp => sp.Appointments, appointment);

            await this.AddToSet(filter, update);
        }

        public async Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            var serviceProviderFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Appointments.Where(
                        appointment => appointment.AppointmentId == new ObjectId(appointmentId)
                    )
                );

            var appointment = await this.GetSingleByFilterAndProject(serviceProviderFilter, project);

            return appointment;
        }

        public async Task<Appointment> GetAppointmentByType(string serviceProviderId, string customerId, AppointmentType appointmentType)
        {
            var serviceProviderFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

            var project = Builders<ServiceProvider>.Projection.Expression(
                sp => sp.Appointments.Where(
                    appointment => (appointment.CustomerId == customerId && appointment.AppointmentType == AppointmentType.CustomerManagement)
                    )
                );

            var appointment = await this.GetSingleByFilterAndProject(serviceProviderFilter, project);

            return appointment;
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

        public async Task CancelAppointment(Appointment appointment)
        {
            var filter = Builders<ServiceProvider>.Filter;

            var nestedFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.Appointments, a => a.AppointmentId.Equals(appointment.AppointmentId));

            var update = Builders<ServiceProvider>.Update.Set(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));

            update = update.Set("Appointments.$.Status", AppointmentStatus.Cancelled);

            update = update.Set("Appointments.$.Cancellation", appointment.Cancellation);

            await this.Upsert(nestedFilter, update);
        }


        // There is no UI for this yet. Will discuss and complete this later
        public async Task RescheduleAppointment(Appointment appointment)
        {
            var filter = Builders<ServiceProvider>.Filter;

            var nestedFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.Appointments, a => a.AppointmentId.Equals(appointment.AppointmentId));

            var update = Builders<ServiceProvider>.Update.Set(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));
            update = update.Set("Appointments.$.ScheduledAppointmentStartTime", appointment.ScheduledAppointmentStartTime);
            update = update.Set("Appointments.$.ScheduledAppointmentEndTime", appointment.ScheduledAppointmentEndTime);

            await this.Upsert(nestedFilter, update);

        }
        public async Task EndAppointment(Appointment appointment)
        {
            var filter = Builders<ServiceProvider>.Filter;

            var nestedFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.Appointments, a => a.AppointmentId.Equals(appointment.AppointmentId));

            var update = Builders<ServiceProvider>.Update.Set(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));
            update = update.Set("Appointments.$.Status", AppointmentStatus.Finished);
            update = update.Set("Appointments.$.ActualAppointmentStartTime", appointment.ActualAppointmentStartTime);
            update = update.Set("Appointments.$.ActualAppointmentEndTime", appointment.ActualAppointmentEndTime);

            await this.Upsert(nestedFilter, update);
        }
    }
}
