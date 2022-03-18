using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace RestApi.Controllers.Provider;

[Route("api/provider/settings_configuration")]
[ApiController]
public class SettingsConfigurationController: ControllerBase
{
    private readonly ISettingsConfigurationService settingsConfigurationService;
    public SettingsConfigurationController(ISettingsConfigurationService settingsConfigurationService)
    {
        this.settingsConfigurationService = settingsConfigurationService;
    }
    
    [HttpGet("{OrganisationId}/{ServiceproviderId}")]
    [Authorize]
    public async Task<ProviderClientOutgoing.SettingsConfigurationOutgoing> GetAllTreatmentPlans(string OrganisationId, string ServiceproviderId)
    {

        var config = await settingsConfigurationService.GetServiceProviderConfig(ServiceproviderId, OrganisationId);

        return config;
    }
}