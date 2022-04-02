using System.Collections.Generic;

namespace DataModel.Mongo.Configuration;

public class FollowupSettings
{
    public bool IsEnabled { get; set; }
    
    public List<string> Reasons { get; set; }
}