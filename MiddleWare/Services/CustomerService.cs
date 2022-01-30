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
using Microsoft.ApplicationInsights.Metrics.Extensibility;

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
        #region GetCustomerProfileRelated
        private async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileInternal(string customerId, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);

                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var customerProfile = await customerRepository.GetCustomerProfile(customerId, organisationId);

                if (customerProfile == null)
                {
                    return null;
                }
                else
                {
                    logger.LogInformation("Begin data conversion ConvertToClientCustomerProfile");

                    var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

                    logger.LogInformation("Finished data conversion ConvertToClientCustomerProfile");

                    return clientCustomer;
                }
            }
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            return await GetCustomerProfileInternal(customerId, organisationId);

        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile?> GetCustomerProfileFromPhoneNumber(string phoneNumber, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);

                DataValidation.ValidateObject(phoneNumber);

                var customer = await customerRepository.GetCustomerFromPhoneNumber(phoneNumber);

                if (customer == null)
                {
                    logger.LogInformation($"No customers for PhoneNumber:{phoneNumber} OrganisationId:{organisationId}");
                    return null;
                }

                return await GetCustomerProfileInternal(customer.CustomerId.ToString(), organisationId);

            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomerProfiles(string organsiationId)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:GetCustomers"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {

                DataValidation.ValidateObjectId(organsiationId, IdType.Organisation);

                var customerProfiles = await customerRepository.GetCustomersOfOrganisation(organsiationId);

                if (customerProfiles == null)
                {
                    logger.LogInformation($"No customers for organisation:{organsiationId}");
                }
                else
                {
                    logger.LogInformation($"Customer count:{customerProfiles.Count} for organisation:{organsiationId}");
                }

                var clientCustomers = CustomerConverter.ConvertToClientCustomerProfileList(customerProfiles);

                return clientCustomers;

            }

        }

        #endregion

        #region AddCustomerProfile
        public async Task<string> AddCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:AddCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                return await AddNewCustomerProfile(customerProfile);
            }
        }

        private async Task<string> AddNewCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            DataValidation.ValidateObject(customerProfile);

            if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
            {
                throw new ArgumentException("No valid phone number passed");
            }

            DataValidation.ValidateObjectId(customerProfile.OrganisationId, IdType.Organisation);

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

            var alreadyExistingProfileWithOrganisation = customer.Profiles.Where(profile => profile.OrganisationId == customerProfile.OrganisationId).Any();

            if (alreadyExistingProfileWithOrganisation)
            {
                throw new ArgumentException("A profile with this organisation already exists");
            }

            customerProfile.CustomerId = customer.CustomerId.ToString();

            logger.LogInformation("Begin data conversion ConvertToMongoCustomerProfile");

            var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

            logger.LogInformation("Finished data conversion ConvertToMongoCustomerProfile");

            await customerRepository.AddCustomerProfile(mongoCustomerProfile);

            return customer.CustomerId.ToString();
        }

        #endregion


        public async Task UpdateCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "CustomerService:UpdateCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                await UpdateExistingCustomerProfile(customerProfile);
            }
        }


        #region Private methods

        private async Task<string> UpdateExistingCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            DataValidation.ValidateObject(customerProfile);

            DataValidation.ValidateObjectId(customerProfile.CustomerId, IdType.Customer);
            DataValidation.ValidateObjectId(customerProfile.OrganisationId, IdType.Organisation);

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

            var alreadyExistingProfileWithOrganisation = customer.Profiles.Where(profile => profile.OrganisationId == customerProfile.OrganisationId).Any();

            if (alreadyExistingProfileWithOrganisation)
            {
                customerProfile.CustomerProfileId = customer.Profiles.Where(
                    profile => profile.OrganisationId == customerProfile.OrganisationId)
                    .First()
                    .CustomerProfileId
                    .ToString();
            }
            else
            {
                throw new Exceptions.InvalidDataException("No profile exists for this organisation");
            }

            logger.LogInformation("Begin data conversion ConvertToMongoCustomerProfile");

            var mongoCustomerProfile = CustomerConverter.ConvertToMongoCustomerProfile(customerProfile);

            logger.LogInformation("Finished data conversion ConvertToMongoCustomerProfile");

            await customerRepository.UpdateCustomerProfile(mongoCustomerProfile);

            return customer.CustomerId.ToString();
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

            await customerRepository.Add(customer);

            return customer;
        }

        #endregion Private methods
    }
}
