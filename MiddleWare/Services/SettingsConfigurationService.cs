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

        var referralWhitelist = new ProviderClientOutgoing.ReferralWhitelistOutgoing();
        if (mongoConfig.ReferralWhitelist != null)
        {
            referralWhitelist.IsEnabled = mongoConfig.ReferralWhitelist.IsEnabled;
            referralWhitelist.ReferralContacts = new List<ProviderClientOutgoing.ReferralContactOutgoing>();
            if(mongoConfig.ReferralWhitelist.ReferralContacts != null)
                foreach (var contact in mongoConfig.ReferralWhitelist.ReferralContacts)
                {
                    var contactOutgoing = new ProviderClientOutgoing.ReferralContactOutgoing();
                    contactOutgoing.ContactName = contact.ContactName;
                    contactOutgoing.PhoneNumber = contact.PhoneNumber;
                    referralWhitelist.ReferralContacts.Add(contactOutgoing);
                }
        }
        
        var followupSettings = new ProviderClientOutgoing.FollowupSettingsOutgoing();
        if (mongoConfig.FollowupSettings != null)
        {
            followupSettings.IsEnabled = mongoConfig.ReferralWhitelist.IsEnabled;
            followupSettings.Reasons = new List<string>();
            if(mongoConfig.FollowupSettings.Reasons != null)
                followupSettings.Reasons.AddRange(mongoConfig.FollowupSettings.Reasons);
        }

        config.FollowupSettings = followupSettings;
        config.AppointmentSettings = appointmentSettingsOutgoing;
        config.ReferralWhitelist = referralWhitelist;

        return config;
    }
}