using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
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
    }
}
