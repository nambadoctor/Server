using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;

namespace MiddleWare.Services
{
    public class CustomerService : ICustomerService
    {
        private IMongoDbDataLayer datalayer;

        public CustomerService(IMongoDbDataLayer dataLayer)
        {
            this.datalayer = dataLayer;
        }
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomer(string customerId, string organisationId)
        {
            var customerProfile = await datalayer.GetCustomerProfile(customerId, organisationId);

            var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

            return clientCustomer;
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomers(string organsiationId, List<string> serviceProviderIds)

        {
            var customerProfiles = await datalayer.GetCustomerProfilesAddedByOrganisation(organsiationId, serviceProviderIds);

            var clientCustomers = new List<ProviderClientOutgoing.OutgoingCustomerProfile>();

            foreach (var customer in customerProfiles)
            {
                clientCustomers.Add(CustomerConverter.ConvertToClientCustomerProfile(customer));
            }

            return clientCustomers;
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> SetCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
            {
                throw new InvalidDataException("No valid phone number passed");
            }

            var generatedCustomerProfile = await datalayer.SetCustomerProfile(CustomerConverter.ConvertToMongoCustomerProfile(customerProfile));

            var clientCustomerProfile = CustomerConverter.ConvertToClientCustomerProfile(generatedCustomerProfile);

            return clientCustomerProfile;
        }

        public async Task<ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing> SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerAddedData)
        {
            if (customerAddedData.PhoneNumbers == null || customerAddedData.PhoneNumbers.Count == 0)
            {
                throw new InvalidDataException("No valid phone number passed");
            }

            var parsedData = GenerateDataForSettingCustomerAndAppointment(customerAddedData);

            var generatedCustomerProfile = await datalayer.SetCustomerWithAppointment(
                parsedData.Item1,
                parsedData.Item2,
                parsedData.Item3
                );

            var clientCustomerProfile = CustomerConverter.ConvertToClientCustomerProfile(generatedCustomerProfile.Item1);

            var spProfile = await datalayer.GetServiceProviderProfile(customerAddedData.ServiceProviderId, customerAddedData.OrganisationId);

            var clientAppointment = AppointmentConverter.ConvertToClientAppointmentData(
                spProfile, generatedCustomerProfile.Item2,
                generatedCustomerProfile.Item1);

            return new ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing(clientCustomerProfile, clientAppointment);
        }

        private (Mongo.CustomerProfile, Mongo.Appointment, Mongo.ServiceRequest) GenerateDataForSettingCustomerAndAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerAddedData)
        {
            var customerProfile = new ProviderClientIncoming.CustomerProfileIncoming();
            customerProfile.OrganisationId = customerAddedData.OrganisationId;
            customerProfile.ServiceProviderId = customerAddedData.ServiceProviderId;
            customerProfile.FirstName = customerAddedData.FirstName;
            customerProfile.LastName = customerAddedData.LastName;
            customerProfile.Gender = customerAddedData.Gender;
            customerProfile.DateOfBirth = customerAddedData.DateOfBirth;
            customerProfile.CustomerId = customerAddedData.CustomerId;
            customerProfile.EmailAddress = customerAddedData.EmailAddress;
            customerProfile.ProfilePicURL = customerAddedData.ProfilePicURL;
            customerProfile.PhoneNumbers = customerProfile.PhoneNumbers;

            var appointment = new Mongo.Appointment();
            var appointmentId = ObjectId.GenerateNewId();
            var serviceRequestId = ObjectId.GenerateNewId();

            appointment.AppointmentId = appointmentId;
            appointment.ServiceRequestId = serviceRequestId.ToString();
            appointment.AddressId = customerAddedData.AddressId;
            appointment.CustomerId = customerAddedData.CustomerId;
            appointment.OrganisationId = customerAddedData.OrganisationId;
            appointment.ScheduledAppointmentStartTime = customerAddedData.ScheduledAppointmentStartTime;
            appointment.ScheduledAppointmentEndTime = customerAddedData.ScheduledAppointmentEndTime;
            appointment.ActualAppointmentStartTime = customerAddedData.ActualAppointmentStartTime;
            appointment.ActualAppointmentEndTime = customerAddedData.ActualAppointmentEndTime;

            var serviceRequest = new Mongo.ServiceRequest();
            serviceRequest.ServiceRequestId = serviceRequestId;
            serviceRequest.AppointmentId = appointmentId.ToString();
            serviceRequest.ServiceProviderId = customerAddedData.ServiceProviderId;
            serviceRequest.OrganisationId = customerAddedData.OrganisationId;

            return (
                CustomerConverter.ConvertToMongoCustomerProfile(customerProfile),
                appointment,
                serviceRequest
                );

        }
    }
}
