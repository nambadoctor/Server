using Client = DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface ICustomerService
    {
        public Task<Client.CustomerProfile> GetCustomer(string customerId, string organisationId);
        public Task<List<Client.CustomerProfile>> GetCustomers(string organsiationId, List<string> serviceProviderIds);
        public Task<Client.CustomerProfile> SetCustomerProfile(Client.CustomerProfile customerProfile);
    }
}
