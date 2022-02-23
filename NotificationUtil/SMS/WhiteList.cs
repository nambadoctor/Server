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
        private static string? envWhiteList = Environment.GetEnvironmentVariable("WHITE_LISTED_DOCTORS");
        public static NotificationWhitelist? SpWhiteList = !string.IsNullOrWhiteSpace(envWhiteList) ? JsonConvert.DeserializeObject<NotificationWhitelist>(envWhiteList) : new NotificationWhitelist();


        public static bool IsServiceproviderWhitelisted(string serviceProviderId)
        {
            var isWhiteListed = false;
            if (SpWhiteList != null)
            {
                foreach (var whiteList in SpWhiteList.whitelist)
                {
                    if (whiteList.Key == serviceProviderId)
                    {
                        if (whiteList.Value.TO_SEND_TO_SELF)
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
            if (SpWhiteList != null)
            {
                foreach (var whiteList in SpWhiteList.whitelist)
                {
                    if (whiteList.Key == serviceProviderId)
                    {
                        if (whiteList.Value.TO_SEND_TO_PATIENTS)
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