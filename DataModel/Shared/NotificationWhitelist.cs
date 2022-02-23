using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared
{
    public class NotificationWhitelist
    {
        public Dictionary<string, WhitelistConfig> whitelist { get; set; }
    }

    public class WhitelistConfig
    {
        public bool TO_SEND_TO_SELF { get; set; }
        public bool TO_SEND_TO_PATIENTS { get; set; }
    }
}
