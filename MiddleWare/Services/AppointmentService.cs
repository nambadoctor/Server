using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IMongoDbDataLayer datalayer;

        public AppointmentService(IMongoDbDataLayer dataLayer)
        {
            this.datalayer = dataLayer;
        }
        public async Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            var appointment = await datalayer.GetAppointment(serviceProviderId, appointmentId);

            var serviceProviderProfile = await datalayer.GetServiceProviderProfile(serviceProviderId, appointment.OrganisationId);

            var customerProfile = await datalayer.GetCustomerProfile(appointmentId, appointment.CustomerId);

            var appointmentData = AppointmentConverter.ConvertToClientAppointmentData(serviceProviderProfile, appointment, customerProfile);

            return appointmentData;
        }

        public async Task<List<Appointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds)
        {
            if (string.IsNullOrWhiteSpace(organsiationId))
            {
                throw new ArgumentException("Organisation Id was null");
            }

            var appointments = await datalayer.GetAppointmentsForServiceProvider(organsiationId, serviceProviderIds);

            var serviceProviderProfiles = await datalayer.GetServiceProviderProfiles(serviceProviderIds, organsiationId);

            //Get customers

            var customerIdsToFetch = new List<string>();

            foreach (var appointment in appointments)
            {
                customerIdsToFetch.Add(appointment.CustomerId);
            }

            var customerProfiles = await datalayer.GetCustomerProfiles(customerIdsToFetch, organsiationId);

            //Piece together all the objects

            var listToReturn = new List<Appointment>();

            foreach (var appointment in appointments)
            {
                var spProfile = (from sp in serviceProviderProfiles
                                 where sp.ServiceProviderId == appointment.ServiceProviderId
                                 select sp).First();

                var custProfile = (from cust in customerProfiles
                                   where cust.CustomerId == appointment.CustomerId
                                   select cust).First();


                listToReturn.Add(AppointmentConverter.ConvertToClientAppointmentData(
                    spProfile,
                    appointment,
                    custProfile)
                    );
            }

            return listToReturn;
        }

    }
}
