using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Interfaces;
using DataModel.Shared;
using MiddleWare.Utils;
using MongoDB.GenericRepository.Interfaces;
using Exceptions = DataModel.Shared.Exceptions;

namespace MiddleWare.Services;

public class SettingsConfigurationService: ISettingsConfigurationService
{
    private ILogger logger;
    private ISettingsConfigurationRepository settingsConfigurationRepository;

    public SettingsConfigurationService(ILogger<SettingsConfigurationService> logger, ISettingsConfigurationRepository settingsConfigurationRepository)
    {
        this.settingsConfigurationRepository = settingsConfigurationRepository;
        this.logger = logger;
    }
    
    public async Task<ProviderClientOutgoing.SettingsConfigurationOutgoing> GetServiceProviderConfig(string ServiceProviderId, string OrganisationId)
    {
        using (logger.BeginScope("Method: {Method}", "SettingsConfigurationService:GetServiceProviderConfig"))
        using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
        {
            DataValidation.ValidateObjectId(ServiceProviderId, IdType.ServiceProvider);
            DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

            var mongoConfig = await settingsConfigurationRepository.GetServiceProviderConfiguration(ServiceProviderId, OrganisationId);

            logger.LogInformation($"Recieved config from db id:{mongoConfig.ConfigurationSettingId.ToString()}");
            var config = ConvertToSettingsConfigurationOutgoing(mongoConfig);
            logger.LogInformation($"Converted config from mongo to outgoing with id:{mongoConfig.ConfigurationSettingId.ToString()}");
            return config;
        }
    }

    private ProviderClientOutgoing.SettingsConfigurationOutgoing ConvertToSettingsConfigurationOutgoing(Mongo.Configuration.SettingsConfiguration mongoConfig)
    {
        var config = new ProviderClientOutgoing.SettingsConfigurationOutgoing();

        config.OrganisationId = mongoConfig.OrganisationId;
        config.ServiceProviderId = mongoConfig.ServiceProviderId;

        var appointmentSettingsOutgoing = new ProviderClientOutgoing.AppointmentSettingsOutgoing();
        if(mongoConfig.AppointmentSettings!=null)
            appointmentSettingsOutgoing.AppointmentReasons = mongoConfig.AppointmentSettings.AppointmentReasons;

        config.AppointmentSettings = appointmentSettingsOutgoing;

        return config;
    }
}