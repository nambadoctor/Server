using System;

namespace DataModel.Client.Provider.Incoming;

public class FollowupIncoming
{
    public string CustomerId { get; set; }
    public string SenderServiceProviderId { get; set; }
    public string OrganisationId { get; set; }
    public string PhoneNumber { get; set; }
    public string Reason { get; set; }
    public DateTime ScheduledDateTime { get; set; }
}