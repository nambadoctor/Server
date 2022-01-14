using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Exceptions = DataModel.Shared.Exceptions;
using Mongo = DataModel.Mongo;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using DataModel.Shared;
using MiddleWare.Utils;
using MongoDB.GenericRepository.Interfaces;

namespace MiddleWare.Services
{
    /*
     * No custotmer and No Customer Profiler
     *  Client calls Inserver Customer 
     *  Server inserts cutomer and customer profile
     *  
     *  Customer Exisits but not profile exists for the given organisaion
     *      Client calls Insert customer
     *      Server finds customerId based on Phonenumber. 
     *      Use above customerId and insert Custormer Profile ( Need to validate there is no profile for that PhoneNUmber+ org combination)
     *  
     *  Customer and Profile Exists
     *      Client calls with CustomerId and CustomerProfileId
     *      easy update
    */

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
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);

                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var customerProfile = await customerRepository.GetCustomerProfile(customerId, organisationId);

                DataValidation.ValidateObject(customerProfile);

                logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile");

                var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                logger.LogInformation("Finished data conversion ConvertToClientCustomerProfile");

                return clientCustomer;
            }
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string phoneNumber, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);

                DataValidation.ValidateObjectId(phoneNumber, IdType.Customer);

                var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

                if (customer == null)
                {
                    throw new Exceptions.ResourceNotFoundException("No customer found for this phone number");
                }

                var customerProfile = await customerRepository.GetCustomerProfile(customer.CustomerId.ToString(), organisationId);

                if (customerProfile == null)
                {
                    throw new Exceptions.ResourceNotFoundException("No customer profile found for this phone number and organisation");
                }

                logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile");

                var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                logger.LogInformation("Finished data conversion ConvertToClientCustomerProfile");

                return clientCustomer;
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomerProfiles(string organsiationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomers"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {

                DataValidation.ValidateObjectId(organsiationId, IdType.Organisation);

                var customerProfiles = await customerRepository.GetCustomersOfOrganisation(organsiationId, serviceProviderIds);

                if (customerProfiles == null)
                {
                    logger.LogInformation($"No customers for organisation:{organsiationId} spIdsCount:{serviceProviderIds.Count}");
                }
                else
                {
                    logger.LogInformation($"Customer count:{customerProfiles.Count} for organisation:{organsiationId} spIdsCount:{serviceProviderIds.Count}");
                }

                var clientCustomers = CustomerConverter.ConvertToClientCustomerProfileList(customerProfiles);

                return clientCustomers;

            }

        }

        public async Task AddCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:AddCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                await AddNewCustomerProfile(customerProfile);
            }
        }

        public async Task UpdateCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:UpdateCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                await UpdateExistingCustomerProfile(customerProfile);
            }
        }

        public async Task SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:SetCustomerProfileWithAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
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

        }

        #region Private methods
        private async Task<string> AddNewCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            DataValidation.ValidateObject(customerProfile);

            if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
            {
                throw new ArgumentException("No valid phone number passed");
            }

            DataValidation.ValidateObjectId(customerProfile.OrganisationId, IdType.Organisation);
            DataValidation.ValidateObjectId(customerProfile.ServiceProviderId, IdType.ServiceProvider);

            if (!string.IsNullOrWhiteSpace(customerProfile.CustomerId))
            {
                throw new ArgumentException("Customer id was not null for customer add");
            }

            var phoneNumber = customerProfile.PhoneNumbers.First().CountryCode + customerProfile.PhoneNumbers.First().Number;

            var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

            //If customer obj didnt exist before, add customer and get back Id
            if (customer == null)
            {
                logger.LogInformation($"New customer set started for phone number : {phoneNumber}");
                customer = await GenerateAndAddNewCustomer(customerProfile.PhoneNumbers.First());
            }

            customerProfile.CustomerId = customer.CustomerId.ToString();

            logger.LogInformation("Begin data conversion ConvertToMongoCustomerProfile");

            var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

            logger.LogInformation("Finished data conversion ConvertToMongoCustomerProfile");

            await customerRepository.AddCustomerProfile(mongoCustomerProfile);

            return customer.CustomerId.ToString();
        }

        private async Task<string> UpdateExistingCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            DataValidation.ValidateObject(customerProfile);

            DataValidation.ValidateObjectId(customerProfile.CustomerId, IdType.Customer);
            DataValidation.ValidateObjectId(customerProfile.OrganisationId, IdType.Organisation);
            DataValidation.ValidateObjectId(customerProfile.ServiceProviderId, IdType.ServiceProvider);

            var phoneNumber = customerProfile.PhoneNumbers.First().CountryCode + customerProfile.PhoneNumbers.First().Number;

            var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

            if (customer == null)
            {
                throw new ArgumentException("Customer does not exist");
            }

            if (customer.CustomerId.ToString() != customerProfile.CustomerId)
            {
                throw new ArgumentException("Customer ID does not match the phone number");
            }

            logger.LogInformation("Begin data conversion ConvertToMongoCustomerProfile");

            var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

            logger.LogInformation("Finished data conversion ConvertToMongoCustomerProfile");

            await customerRepository.UpdateCustomerProfile(mongoCustomerProfile);

            return customer.CustomerId.ToString();
        }

        private async Task<string> UpsertCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            if (string.IsNullOrWhiteSpace(customerProfile.CustomerId))
            {
                //Assume New customer
                return await AddNewCustomerProfile(customerProfile);
            }
            else
            {
                return await UpdateExistingCustomerProfile(customerProfile);
            }
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

        #endregion Private methods
    }
}
