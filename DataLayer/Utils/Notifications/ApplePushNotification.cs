using CorePush.Apple;
using Newtonsoft.Json;
using DataModel.Mongo;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.Shared;

namespace DataLayer.Notifications
{
    public class AppleNotification
    {
        public class ApsPayload
        {
            public class Alert
            {
                [JsonProperty("title")]
                public string Title { get; set; }

                [JsonProperty("body")]
                public string Body { get; set; }

                [JsonProperty("type")]
                public string NotifType { get; set; }

                [JsonProperty("id")]
                public string id { get; set; }

            }

            [JsonProperty("alert")]
            public Alert AlertBody { get; set; }

            [JsonProperty("apns-push-type")]
            public string PushType { get; set; } = "alert";

            [JsonProperty("sound")]
            public string Sound { get; set; } = "default";
        }

        public AppleNotification(Guid id, string message, string title = "", string notifType = "", string appointmentId = "")
        {
            Id = id;

            Aps = new ApsPayload
            {
                AlertBody = new ApsPayload.Alert
                {
                    Title = title,
                    Body = message,
                    NotifType = notifType,
                    id = appointmentId
                }
            };
        }

        [JsonProperty("aps")]
        public ApsPayload Aps { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
    public class ApplePushNotification
    {
        private string apnBundleId = "";
        private string apnP8PrivateKey = "";
        private string apnP8PrivateKeyId = "";
        private string apnTeamId = "";
        private ApnServerType apnServerType = ApnServerType.Development;
        private static readonly HttpClient http = new HttpClient();

        private INDLogger _NDLogger;

        public ApplePushNotification(INDLogger NDLogger)
        {

            _NDLogger = NDLogger;

            this.apnBundleId = ConnectionConfiguration.ApnBundleId;
            this.apnP8PrivateKey = ConnectionConfiguration.ApnP8PrivateKey;
            this.apnP8PrivateKeyId = ConnectionConfiguration.ApnP8PrivateKeyId;
            this.apnTeamId = ConnectionConfiguration.ApnTeamId;
            this.apnServerType = (ApnServerType)ConnectionConfiguration.ApnsServiceType;
        }

        public async Task<bool> SendApnNotificationAsync(string deviceToken, string title, string body, string notifType, string appointmentId)
        {

            _NDLogger.LogEvent($"APNS SERVICE TYPE {apnServerType}");

            var settings = new ApnSettings
            {
                AppBundleIdentifier = apnBundleId,
                P8PrivateKey = apnP8PrivateKey,
                P8PrivateKeyId = apnP8PrivateKeyId,
                TeamId = apnTeamId,
                ServerType = apnServerType,
            };

            string s = File.ReadAllText(@"AuthKey_UBQBAA2LX7.p8");
            settings.P8PrivateKey = s;

            var apn = new ApnSender(settings, http);
            var payload = new AppleNotification(
                Guid.NewGuid(),
                body,
                title,
                notifType,
                appointmentId);
            var response = await apn.SendAsync(payload, deviceToken);

            if (response.Error != null)
            {
                _NDLogger.LogEvent($"APNS NOTIFICATION ERROR RESPONSE: {response.Error.Reason}");
            }

            return response.IsSuccess;
        }

    }
}
