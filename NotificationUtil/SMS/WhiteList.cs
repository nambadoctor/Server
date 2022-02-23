using DataModel.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Sms
{
    public static class WhiteList
    {
        private static string? env_whitelist = Environment.GetEnvironmentVariable("WHITE_LISTED_DOCTORS");
        private static Dictionary<string, WhitelistConfig>? notificationWhitelist = env_whitelist != null ? JsonConvert.DeserializeObject<Dictionary<string, WhitelistConfig>>(env_whitelist) : new Dictionary<string, WhitelistConfig>();

        public static bool IsServiceproviderWhitelisted(string serviceProviderId)
        {
            var isWhiteListed = false;
            if (notificationWhitelist != null)
            {
                foreach (var pair in notificationWhitelist)
                {
                    if (pair.Key == serviceProviderId)
                    {
                        if (pair.Value.TO_SEND_TO_SELF)
                        {
                            isWhiteListed = true;
                            break;
                        }
                    }
                }
            }
            return isWhiteListed;
        }

        public static bool IsCustomerWhitelistedByServiceprovider(string serviceProviderId)
        {
            var isWhiteListed = false;
            if (notificationWhitelist != null)
            {
                foreach (var pair in notificationWhitelist)
                {
                    if (pair.Key == serviceProviderId)
                    {
                        if (pair.Value.TO_SEND_TO_PATIENTS)
                        {
                            isWhiteListed = true;
                            break;
                        }
                    }
                }
            }
            return isWhiteListed;
        }
    }
}