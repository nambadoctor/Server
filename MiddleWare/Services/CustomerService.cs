using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using DataModel.Shared;
using DataModel.Shared.Exceptions;
using MiddleWare.Utils;

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
                    DataValidation.ValidateIncomingId(organisationId, IdType.Organisation);

                    DataValidation.ValidateIncomingId(customerId, IdType.Customer);

                    var customerProfile = await datalayer.GetCustomerProfile(customerId, organisationId);

                    DataValidation.ValidateObject(customerProfile);

                    logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile");

                    var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                    logger.LogInformation("Finished data conversion ConvertToClientCustomerProfile");

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
                    DataValidation.ValidateIncomingId(organisationId, IdType.Organisation);

                    DataValidation.ValidateIncomingId(phoneNumber, IdType.Customer);

                    var customer = await datalayer.GetCustomerFromRegisteredPhoneNumber(phoneNumber);

                    if (customer != null)
                    {
                        return new ProviderClientOutgoing.OutgoingCustomerProfile();
                    }

                    var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(phoneNumber);

                    if (serviceProvider != null)
                    {
                        throw new PhoneNumberBelongsToServiceProviderException($"Phone number : {phoneNumber} belongs to Service provider {serviceProvider.ServiceProviderId}");
                    }

                    var customerProfile = await datalayer.GetCustomerProfile(customer.CustomerId.ToString(), organisationId);

                    if (customerProfile == null)
                    {
                        throw new CustomerDoesNotExistException($"Customer does not exist for id: {customer.CustomerId} org id: {organisationId}");
                    }

                    logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile");

                    var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                    logger.LogInformation("Finished data conversion ConvertToClientCustomerProfile");

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
                    DataValidation.ValidateIncomingId(organsiationId, IdType.Organisation);

                    var customerProfiles = await datalayer.GetCustomerProfilesAddedByOrganisation(organsiationId, serviceProviderIds);

                    if (customerProfiles == null)
                    {
                        logger.LogInformation($"No customers for organisation:{organsiationId} spIdsCount:{serviceProviderIds.Count}");
                    }
                    else
                    {
                        logger.LogInformation($"Customer count:{customerProfiles.Count} for organisation:{organsiationId} spIdsCount:{serviceProviderIds.Count}");
                    }

                    var clientCustomers = new List<ProviderClientOutgoing.OutgoingCustomerProfile>();

                    if (customerProfiles != null)
                    {
                        logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile list");

                        foreach (var customer in customerProfiles)
                        {
                            clientCustomers.Add(CustomerConverter.ConvertToClientCustomerProfile(customer));
                        }

                        logger.LogInformation("End data conversion ConvertToClientCustomerProfile list");
                    }

                    return clientCustomers;
                }
                finally
                {

                }
            }

        }

        public async Task SetCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
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

                    var phoneNumber = customerProfile.PhoneNumbers.First().CountryCode + customerProfile.PhoneNumbers.First().Number;

                    var customer = await datalayer.GetCustomerFromRegisteredPhoneNumber(phoneNumber);

                    if (customer == null)
                    {
                        logger.LogInformation($"New customer set started for phone number : {phoneNumber}");
                    }

                    //Update customer
                    if (!string.IsNullOrWhiteSpace(customerProfile.CustomerId) && ObjectId.TryParse(customerProfile.CustomerId, out ObjectId customerId) == false)
                    {
                        logger.LogInformation($"Found existing customer for phone number : {phoneNumber}");
                        customerProfile.CustomerId = customerId.ToString();
                    }

                    logger.LogInformation("Begin data conversion ConvertToMongoCustomerProfile");

                    var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

                    logger.LogInformation("Finished data conversion ConvertToMongoCustomerProfile");

                    await datalayer.SetCustomerProfile(mongoCustomerProfile);

                }
                finally
                {

                }
            }

        }

        public async Task SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:SetCustomerProfileWithAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    var customerProfile = customerProfileWithAppointmentIncoming.CustomerProfileIncoming;

                    if(customerProfile == null)
                    {
                        throw new ArgumentException("Null customer passed");
                    }

                    if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
                    {
                        throw new ArgumentException("No valid phone number passed");
                    }

                    DataValidation.ValidateIncomingId(customerProfile.OrganisationId, IdType.Organisation);
                    DataValidation.ValidateIncomingId(customerProfile.ServiceProviderId, IdType.ServiceProvider);

                    var phoneNumber = customerProfile.PhoneNumbers.First().CountryCode + customerProfile.PhoneNumbers.First().Number;

                    var customer = await datalayer.GetCustomerFromRegisteredPhoneNumber(phoneNumber);

                    if (customer == null)
                    {
                        logger.LogInformation($"New customer set started for phone number : {phoneNumber}");
                    }
                    else
                    {
                        logger.LogInformation($"Found existing customer for phone number : {phoneNumber}");
                        customerProfile.CustomerId = customer.CustomerId.ToString();
                    }

                    logger.LogInformation("Begin data conversion GenerateDataForSettingCustomerAndAppointment");

                    var spProfile = await datalayer.GetServiceProviderProfile(
                        customerProfileWithAppointmentIncoming.AppointmentIncoming.ServiceProviderId,
                        customerProfileWithAppointmentIncoming.AppointmentIncoming.OrganisationId);

                    DataValidation.ValidateObject(spProfile);

                    var parsedData = GenerateDataForSettingCustomerAndAppointment(customerProfileWithAppointmentIncoming, spProfile);

                    logger.LogInformation("Finished data conversion GenerateDataForSettingCustomerAndAppointment");

                    await datalayer.SetCustomerWithAppointment(
                        parsedData.Item1,
                        parsedData.Item2,
                        parsedData.Item3
                        );
                }
                finally
                {

                }
            }

        }

        private (Mongo.CustomerProfile, Mongo.Appointment, Mongo.ServiceRequest) GenerateDataForSettingCustomerAndAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointmentIncoming, Mongo.ServiceProviderProfile serviceProviderProfile)
        {
            var customerData = customerProfileWithAppointmentIncoming.CustomerProfileIncoming;
            var appointmentData = customerProfileWithAppointmentIncoming.AppointmentIncoming;

            var customerProfile = new ProviderClientIncoming.CustomerProfileIncoming();
            customerProfile.OrganisationId = customerData.OrganisationId;
            customerProfile.ServiceProviderId = customerData.ServiceProviderId;
            customerProfile.FirstName = customerData.FirstName;
            customerProfile.LastName = customerData.LastName;
            customerProfile.Gender = customerData.Gender;
            customerProfile.DateOfBirth = customerData.DateOfBirth;
            customerProfile.CustomerId = customerData.CustomerId;
            customerProfile.PhoneNumbers = customerData.PhoneNumbers;

            var appointment = new Mongo.Appointment();
            var appointmentId = ObjectId.GenerateNewId();
            var serviceRequestId = ObjectId.GenerateNewId();

            appointment.AppointmentId = appointmentId;
            appointment.ServiceProviderId = appointmentData.ServiceProviderId;
            appointment.ServiceRequestId = serviceRequestId.ToString();
            appointment.CustomerId = appointmentData.CustomerId;
            appointment.OrganisationId = appointmentData.OrganisationId;
            appointment.ScheduledAppointmentStartTime = appointmentData.ScheduledAppointmentStartTime;
            appointment.ScheduledAppointmentEndTime = appointmentData.ScheduledAppointmentEndTime;
            appointment.ActualAppointmentStartTime = appointmentData.ActualAppointmentStartTime;
            appointment.ActualAppointmentEndTime = appointmentData.ActualAppointmentEndTime;
            appointment.ServiceProviderName = $"Dr. {serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";
            appointment.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

            var serviceRequest = new Mongo.ServiceRequest();
            serviceRequest.ServiceRequestId = serviceRequestId;
            serviceRequest.AppointmentId = appointmentId.ToString();
            serviceRequest.ServiceProviderId = appointmentData.ServiceProviderId;
            serviceRequest.OrganisationId = appointmentData.OrganisationId;
            serviceRequest.CustomerId = appointmentData.CustomerId;

            return (
                CustomerConverter.ConvertToMongoCustomerProfile(customerProfile),
                appointment,
                serviceRequest
                );

        }
    }
}
