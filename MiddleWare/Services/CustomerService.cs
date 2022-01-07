using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using DataModel.Shared;
using DataModel.Shared.Exceptions;

namespace MiddleWare.Services
{
    public class CustomerService : ICustomerService
    {
        private IMongoDbDataLayer datalayer;
        private ILogger logger;

        public CustomerService(IMongoDbDataLayer dataLayer, ILogger<CustomerService> logger)
        {
            this.datalayer = dataLayer;
            this.logger = logger;
        }
        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    var customerProfile = await datalayer.GetCustomerProfile(customerId, organisationId);

                    var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                    return clientCustomer;
                }
                finally
                {

                }
            }
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string phoneNumber, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    var customer = await datalayer.GetCustomerFromRegisteredPhoneNumber(phoneNumber);

                    if (customer == null)
                    {
                        return null;
                    }

                    var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(phoneNumber);

                    if (serviceProvider == null)
                    {
                        throw new PhoneNumberBelongsToServiceProviderException($"Phone number : {phoneNumber} belongs to Service provider {serviceProvider.ServiceProviderId}");
                    }

                    var customerProfile = await datalayer.GetCustomerProfile(phoneNumber, organisationId);

                    var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                    return clientCustomer;
                }
                finally
                {

                }
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomerProfiles(string organsiationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomers"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    var customerProfiles = await datalayer.GetCustomerProfilesAddedByOrganisation(organsiationId, serviceProviderIds);

                    var clientCustomers = new List<ProviderClientOutgoing.OutgoingCustomerProfile>();

                    foreach (var customer in customerProfiles)
                    {
                        clientCustomers.Add(CustomerConverter.ConvertToClientCustomerProfile(customer));
                    }

                    return clientCustomers;
                }
                finally
                {

                }
            }

        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> SetCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:SetCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
                    {
                        throw new ArgumentException("No valid phone number passed");
                    }

                    var generatedCustomerProfile = await datalayer.SetCustomerProfile(CustomerConverter.ConvertToMongoCustomerProfile(customerProfile));

                    var clientCustomerProfile = CustomerConverter.ConvertToClientCustomerProfile(generatedCustomerProfile);

                    return clientCustomerProfile;
                }
                finally
                {

                }
            }

        }

        public async Task<ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing> SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerAddedData)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:SetCustomerProfileWithAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (customerAddedData.PhoneNumbers == null || customerAddedData.PhoneNumbers.Count == 0)
                    {
                        throw new ArgumentException("No valid phone number passed");
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
                finally
                {

                }
            }

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
            customerProfile.PhoneNumbers = customerAddedData.PhoneNumbers;

            var appointment = new Mongo.Appointment();
            var appointmentId = ObjectId.GenerateNewId();
            var serviceRequestId = ObjectId.GenerateNewId();

            appointment.AppointmentId = appointmentId;
            appointment.ServiceProviderId = customerAddedData.ServiceProviderId;
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
            serviceRequest.CustomerId = customerAddedData.CustomerId;

            return (
                CustomerConverter.ConvertToMongoCustomerProfile(customerProfile),
                appointment,
                serviceRequest
                );

        }
    }
}
