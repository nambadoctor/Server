using DataModel.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IServiceProviderRepository : IRepository<ServiceProvider>
    {
        public Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(string organisationId, List<string> serviceProviderIds);
        public Task<List<ServiceProviderProfile>> GetServiceProviderProfilesOfOrg(string organisationId);
        public Task<ServiceProviderProfile> GetServiceProviderProfile(string serviceProviderId, string organisationId);
        public Task<ServiceProvider> GetServiceProviderFromPhoneNumber(string phoneNumber);

        //Not for Rest API
        public Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(List<string> serviceProviderIds);
    }
}
