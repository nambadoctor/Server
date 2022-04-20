using System.Collections.Generic;

namespace DataModel.Client.Provider.Outgoing;

public class FollowupSettingsOutgoing
{
    public bool IsEnabled { get; set; }
    
    public List<string> Reasons { get; set; }
}