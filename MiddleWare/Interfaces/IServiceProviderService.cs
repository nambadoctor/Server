using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemberships();
        public Task<ProviderClientOutgoing.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId);
    }
}
