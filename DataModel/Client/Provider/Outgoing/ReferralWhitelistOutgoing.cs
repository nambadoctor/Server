using System.Collections.Generic;

namespace DataModel.Client.Provider.Outgoing;

public class ReferralWhitelistOutgoing
{
    public bool IsEnabled { get; set; }
    public List<ReferralContactOutgoing> ReferralContacts { get; set; }
}