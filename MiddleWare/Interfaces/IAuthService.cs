using DataModel.Mongo;

namespace MiddleWare.Interfaces
{
    public interface IAuthService
    {
        public Task<Customer> GetCustomerFromRegisteredPhoneNumber();

        public Task<DataModel.Mongo.ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber();

        public Task<string?> GetDefaultOrganisationId();
    }
}
