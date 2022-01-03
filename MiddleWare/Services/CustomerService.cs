using DataLayer;
using DataModel.Client.Provider;
using MiddleWare.Converters;
using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class CustomerService : ICustomerService
    {
        private IMongoDbDataLayer datalayer;

        public CustomerService(IMongoDbDataLayer dataLayer)
        {
            this.datalayer = dataLayer;
        }
        public async Task<CustomerProfile> GetCustomer(string customerId, string organisationId)
        {
            var customerProfile = await datalayer.GetCustomerProfile(customerId, organisationId);

            var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customerProfile);

            return clientCustomer;
        }

        public async Task<List<CustomerProfile>> GetCustomers(string organsiationId, List<string> serviceProviderIds)

        {
            var customerProfiles = await datalayer.GetCustomerProfilesAddedByOrganisation(organsiationId, serviceProviderIds);

            var clientCustomers = new List<CustomerProfile>();

            foreach (var customer in customerProfiles)
            {
                clientCustomers.Add(CustomerConverter.ConvertToClientCustomerProfile(customer));
            }

            return clientCustomers;
        }

        public async Task<CustomerProfile> SetCustomerProfile(CustomerProfile customerProfile)
        {
            if (customerProfile.PhoneNumbers == null || customerProfile.PhoneNumbers.Count == 0)
            {
                throw new InvalidDataException("No valid phone number passed");
            }

            var generatedCustomerProfile = await datalayer.SetCustomerProfile(CustomerConverter.ConvertToMongoCustomerProfile(customerProfile));

            var clientCustomerProfile = CustomerConverter.ConvertToClientCustomerProfile(generatedCustomerProfile);

            return clientCustomerProfile;
        }
    }
}
