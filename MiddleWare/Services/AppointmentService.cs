using DataLayer;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IMongoDbDataLayer datalayer;

        public AppointmentService(IMongoDbDataLayer dataLayer)
        {
            this.datalayer = dataLayer;
        }
        public async Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            if (string.IsNullOrWhiteSpace(serviceProviderId) || ObjectId.TryParse(serviceProviderId, out ObjectId spId) == false)
            {
                throw new ArgumentException("Service provider Id was invalid");
            }

            if (string.IsNullOrWhiteSpace(appointmentId) || ObjectId.TryParse(appointmentId, out ObjectId appId) == false)
            {
                throw new ArgumentException("Appointment Id was invalid");
            }

            var appointment = await datalayer.GetAppointment(serviceProviderId, appointmentId);

            var serviceProviderProfile = await datalayer.GetServiceProviderProfile(serviceProviderId, appointment.OrganisationId);

            var customerProfile = await datalayer.GetCustomerProfile(appointmentId, appointment.CustomerId);

            var appointmentData = AppointmentConverter.ConvertToClientAppointmentData(serviceProviderProfile, appointment, customerProfile);

            return appointmentData;
        }

        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds)
        {
            if (string.IsNullOrWhiteSpace(organsiationId) || ObjectId.TryParse(organsiationId, out ObjectId orgId) == false)
            {
                throw new ArgumentException("Organisation Id was invalid");
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

            var listToReturn = new List<ProviderClientOutgoing.OutgoingAppointment>();

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

        public async Task<ProviderClientOutgoing.OutgoingAppointment> SetAppointment(ProviderClientIncoming.Appointment appointment)
        {
            //Do validations here for cust, org and service provider id
            if (string.IsNullOrWhiteSpace(appointment.OrganisationId) || ObjectId.TryParse(appointment.OrganisationId, out ObjectId orgId) == false)
            {
                throw new ArgumentException("Organisation Id was invalid");
            }

            if (string.IsNullOrWhiteSpace(appointment.ServiceProviderId) || ObjectId.TryParse(appointment.ServiceProviderId, out ObjectId spId) == false)
            {
                throw new ArgumentException("Service provider Id was invalid");
            }

            if (string.IsNullOrWhiteSpace(appointment.CustomerId) || ObjectId.TryParse(appointment.CustomerId, out ObjectId custId) == false)
            {
                throw new ArgumentException("Customer Id was invalid");
            }
            //Here appointment id is allowed to be null but if not then throw error if invalid id
            if (!string.IsNullOrWhiteSpace(appointment.AppointmentId) && ObjectId.TryParse(appointment.AppointmentId, out ObjectId appId) == false)
            {
                throw new ArgumentException("Appointment Id was invalid");
            }

            var spProfile = await datalayer.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

            if (spProfile == null)
            {
                throw new InvalidDataException("Service provider profile is missing");
            }

            var customerProfile = await datalayer.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

            if (customerProfile == null)
            {
                throw new InvalidDataException("Customer profile is missing");
            }

            //New appointment
            if (string.IsNullOrWhiteSpace(appointment.AppointmentId))
            {
                var serviceRequest = new Mongo.ServiceRequest();

                //Service request and appointment both need Ids
                var appointmentId = ObjectId.GenerateNewId();
                var serviceRequestId = ObjectId.GenerateNewId();

                appointment.AppointmentId = appointmentId.ToString();
                appointment.ServiceRequestId = serviceRequestId.ToString();

                serviceRequest.ServiceRequestId = serviceRequestId;
                serviceRequest.AppointmentId = appointmentId.ToString();

                var generatedAppointment = await datalayer.SetAppointmentWithServiceRequest(
                    AppointmentConverter.ConvertToMongoAppointmentData(appointment),
                    serviceRequest
                    );

                var clientAppointment = AppointmentConverter.ConvertToClientAppointmentData(
                    $"{spProfile.FirstName} {spProfile.LastName}",
                    generatedAppointment,
                    $"{customerProfile.FirstName} {customerProfile.LastName}");

                return clientAppointment;
            }
            else
            {
                //Existing appointment update
                var generatedAppointment = await datalayer.SetAppointment(AppointmentConverter.ConvertToMongoAppointmentData(appointment));

                var clientAppointment = AppointmentConverter.ConvertToClientAppointmentData(
                    $"{spProfile.FirstName} {spProfile.LastName}",
                    generatedAppointment,
                    $"{customerProfile.FirstName} {customerProfile.LastName}");

                return clientAppointment;
            }
        }
    }
}
