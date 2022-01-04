using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IOrganisationService
    {
        /// <summary>
        /// Get organisation based on Id
        /// </summary>
        /// <returns></returns>
        public Task<ProviderClientOutgoing.Organisation> GetOrganisationAsync(string OrganisationId);
    }
}
