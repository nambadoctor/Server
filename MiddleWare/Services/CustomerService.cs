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
using MongoDB.GenericRepository.Interfaces;

namespace MiddleWare.Services
{
    public class CustomerService : ICustomerService
    {
        private ILogger logger;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmenRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private IServiceProviderRepository serviceProviderRepository;

        public CustomerService(ICustomerRepository customerRepository, IServiceProviderRepository serviceProviderRepository, IAppointmentRepository appointmenRepository, IServiceRequestRepository serviceRequestRepository, ILogger<CustomerService> logger)
        {
            this.serviceProviderRepository = serviceProviderRepository;
            this.appointmenRepository = appointmenRepository;
            this.serviceRequestRepository = serviceRequestRepository;
            this.customerRepository = customerRepository;
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

                    var customerProfile = await customerRepository.GetCustomerProfile(customerId, organisationId);

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

                    var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

                    if (customer != null)
                    {
                        return new ProviderClientOutgoing.OutgoingCustomerProfile();
                    }

                    var serviceProvider = await serviceProviderRepository.GetServiceProviderFromPhoneNumber(phoneNumber);

                    if (serviceProvider != null)
                    {
                        throw new PhoneNumberBelongsToServiceProviderException($"Phone number : {phoneNumber} belongs to Service provider {serviceProvider.ServiceProviderId}");
                    }

                    var customerProfile = await customerRepository.GetCustomerProfile(customer.CustomerId.ToString(), organisationId);

                    if (customerProfile == null)
                    {
                        return new ProviderClientOutgoing.OutgoingCustomerProfile { CustomerId = customer.CustomerId.ToString() };
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

                    var customerProfiles = await customerRepository.GetCustomersOfOrganisation(organsiationId, serviceProviderIds);

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

                    await UpsertCustomerProfile(customerProfile);

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

                    var spProfile = await serviceProviderRepository.GetServiceProviderProfile(
                        customerProfileWithAppointmentIncoming.AppointmentIncoming.ServiceProviderId,
                        customerProfileWithAppointmentIncoming.AppointmentIncoming.OrganisationId);

                    DataValidation.ValidateObject(spProfile);

                    var customerProfile = customerProfileWithAppointmentIncoming.CustomerProfileIncoming;

                    var customerId = await UpsertCustomerProfile(customerProfile);

                    logger.LogInformation("Begin data conversion GenerateAppointmentAndServiceRequest");

                    var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

                    var parsedData = GenerateAppointmentAndServiceRequest(customerProfileWithAppointmentIncoming.AppointmentIncoming, mongoCustomerProfile, spProfile);

                    logger.LogInformation("Finished data conversion GenerateAppointmentAndServiceRequest");

                    parsedData.Item1.CustomerId = customerId;
                    parsedData.Item2.CustomerId = customerId;

                    await appointmenRepository.AddAppointment(parsedData.Item1);

                    await serviceRequestRepository.AddServiceRequest(parsedData.Item2);
                }
                finally
                {

                }
            }

        }

        private async Task<string> UpsertCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            DataValidation.ValidateObject(customerProfile);

            if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
            {
                throw new ArgumentException("No valid phone number passed");
            }

            DataValidation.ValidateIncomingId(customerProfile.OrganisationId, IdType.Organisation);
            DataValidation.ValidateIncomingId(customerProfile.ServiceProviderId, IdType.ServiceProvider);

            var phoneNumber = customerProfile.PhoneNumbers.First().CountryCode + customerProfile.PhoneNumbers.First().Number;

            var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

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

            if (customer == null)
            {
                customer = await GenerateAndAddNewCustomer(customerProfile.PhoneNumbers.First());

                mongoCustomerProfile.CustomerId = customer.CustomerId.ToString();
                await customerRepository.AddCustomerProfile(mongoCustomerProfile);
            }
            else
            {
                await customerRepository.UpdateCustomerProfile(mongoCustomerProfile);
            }

            return mongoCustomerProfile.CustomerId;
        }

        private async Task<Mongo.Customer> GenerateAndAddNewCustomer(DataModel.Client.Provider.Common.PhoneNumber phoneNumber)
        {
            var customer = new Mongo.Customer();
            customer.CustomerId = ObjectId.GenerateNewId();
            customer.AuthInfos = new List<Mongo.AuthInfo>();

            var authInfo = new Mongo.AuthInfo();
            authInfo.AuthInfoId = ObjectId.GenerateNewId();
            authInfo.AuthId = phoneNumber.CountryCode + phoneNumber.Number;
            authInfo.AuthType = "PhoneNumber";

            customer.AuthInfos.Add(authInfo);
            customer.Profiles = new List<Mongo.CustomerProfile>();
            customer.ServiceRequests = new List<Mongo.ServiceRequest>();

            await customerRepository.Add(customer);

            return customer;
        }

        private (Mongo.Appointment, Mongo.ServiceRequest) GenerateAppointmentAndServiceRequest(ProviderClientIncoming.AppointmentIncoming appointmentData, Mongo.CustomerProfile customerProfile, Mongo.ServiceProviderProfile serviceProviderProfile)
        {

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
                appointment,
                serviceRequest
                );

        }
    }
}
