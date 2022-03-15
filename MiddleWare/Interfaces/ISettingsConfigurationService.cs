using DataModel.Client.Provider.Outgoing;

namespace MiddleWare.Interfaces;

public interface ISettingsConfigurationService
{
    public Task<SettingsConfigurationOutgoing>
        GetServiceProviderConfig(string ServiceProviderId, string OrganisationId);
}