using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IMongoDbDataLayer datalayer;
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger NDLogger;

        public AppointmentService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.datalayer = dataLayer;
            this.nambaDoctorContext = nambaDoctorContext;
            NDLogger = nambaDoctorContext._NDLogger;
        }
        public async Task<AppointmentData> GetAppointment(string serviceProviderId, string appointmentId)
        {
            var appointmentInformation = await datalayer.GetAppointmentData(serviceProviderId, appointmentId);

            var appointmentData = GetAppointmentDataObject(appointmentInformation);

            return appointmentData;
        }

        public async Task<List<AppointmentData>> GetAppointments(string organsiationId, List<string> serviceProviderIds)
        {
            var appointmentInformations = await datalayer.GetAppointmentsForServiceProvider(organsiationId, serviceProviderIds);

            var listToReturn = new List<AppointmentData>();

            foreach (var appointmentInformation in appointmentInformations)
            {
                listToReturn.Add(GetAppointmentDataObject(appointmentInformation));
            }

            return listToReturn;
        }

        private AppointmentData GetAppointmentDataObject((Mongo.ServiceProvider, Mongo.Appointment, Mongo.Customer, Mongo.ServiceRequest) appointmentInformation)
        {
            var spId = appointmentInformation.Item1.ServiceProviderId.ToString();

            var spProfile = (from profile in appointmentInformation.Item1.Profiles
                             where profile.OrganisationId == appointmentInformation.Item2.OrganisationId
                             select profile).FirstOrDefault();

            if (spProfile == null)
            {
                throw new KeyNotFoundException($"No service provider Id:{spId} profile found for organisation:{appointmentInformation.Item2.OrganisationId}");
            }

            var appointment = appointmentInformation.Item2;

            var custId = appointmentInformation.Item3.CustomerId.ToString();

            var customerProfile = (from profile in appointmentInformation.Item3.Profiles
                                   where profile.OrganisationId == appointmentInformation.Item2.OrganisationId
                                   select profile).FirstOrDefault();

            if (customerProfile == null)
            {
                throw new KeyNotFoundException($"No customer Id:{custId} profile found for organisation:{appointmentInformation.Item2.OrganisationId}");
            }

            var serviceRequest = appointmentInformation.Item4;

            var appointmentData = AppointmentConverter.ConvertToClientAppointmentData(
                spId,
                spProfile,
                appointment,
                custId,
                customerProfile,
                serviceRequest
                );

            return appointmentData;
        }
    }
}
