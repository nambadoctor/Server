namespace DataModel.Client.Provider.Outgoing;

public class SettingsConfigurationOutgoing
{
    public string OrganisationId { get; set; }
    public string ServiceProviderId { get; set; }
    public AppointmentSettingsOutgoing AppointmentSettings { get; set; }
    public ReferralWhitelistOutgoing ReferralWhitelist { get; set; }
    
    public FollowupSettingsOutgoing FollowupSettings { get; set; }
}