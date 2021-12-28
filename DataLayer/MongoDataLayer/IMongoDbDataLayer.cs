using DataModel.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IMongoDbDataLayer
    {
        public Task<ServiceProvider> GetServiceProvider(string serviceProviderId);
        public Task<List<ServiceProvider>> GetServiceProviders(List<string> serviceProviderIds);
        public Task<Organisation> GetOrganisation(string organisationId);
        public Task<List<Organisation>> GetOrganisations(string serviceProviderId);
        public Task<string> GetUserTypeFromRegisteredPhoneNumber(string phoneNumber);
        public Task<Customer> GetCustomerFromRegisteredPhoneNumber(string phoneNumber);
        public Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber);
    }
}
