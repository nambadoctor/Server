using DataModel.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IServiceProviderRepository : IRepository<ServiceProvider>
    {
        public Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(string organisationId, List<string> serviceProviderIds);
        public Task<ServiceProviderProfile> GetServiceProviderProfile(string organisationId, string serviceProviderId);
        public Task<ServiceProvider> GetServiceProviderFromPhoneNumber(string phoneNumber);
    }
}
