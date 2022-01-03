using DataModel.Client.Provider;
using Client = DataModel.Client;

namespace MiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<Client.Provider.ServiceProviderBasic> GetServiceProviderOrganisationMemeberships();
        public Task<Client.Provider.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId);
        public Task<List<Client.GeneratedSlot>> GetServiceProviderSlots(string ServiceProviderId, string OrganisationId);
    }
}
