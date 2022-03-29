using System.Collections.Generic;

namespace DataModel.Mongo.Configuration;

public class ReferralWhitelist
{
    public bool IsEnabled { get; set; }
    public List<ReferralContact> ReferralContacts { get; set; }
}