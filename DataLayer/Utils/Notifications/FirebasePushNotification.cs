using Nancy.Json;
using Newtonsoft.Json;
using DataModel.Mongo;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DataModel.Shared;

namespace DataLayer.Notifications
{
    public class FirebasePushNotification
    {
        private string serverKey = "";
        private string senderId = "";
        private string webAddr = "https://fcm.googleapis.com/fcm/send";

        public FirebasePushNotification()
        {
            this.serverKey = ConnectionConfiguration.FcmServerKey;
            this.senderId = ConnectionConfiguration.FcmSenderId;
        }

        public async Task<bool> SendNotification(string DeviceToken, string title, string msg, string data, string type)
        {
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            httpWebRequest.Method = "POST";

            var ttlSetting = "4500s";
            var prioritySetting = "normal";
            if (type.Equals("CallInRoom"))
            {
                ttlSetting = "0s";
                prioritySetting = "high";
            }
            else if (type.Equals("NewChatMessage"))
            {
                prioritySetting = "normal";
            }
            else if (type.Equals("AppointmentBooked") || type.Equals("AppointmentCancelled"))
            {
                prioritySetting = "high";
            }

            var payload = new
            {
                to = DeviceToken,
                priority = prioritySetting,
                content_available = true,
                data = new
                {
                    id = data,
                    body = msg,
                    title = title,
                    type = type
                },
                analytics_label = type,
                android = new
                {
                    ttl = ttlSetting
                }
            };
            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            try
            {
                var httpResponse = await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                var formattedFcmResponse = JsonConvert.DeserializeObject<FCMServerResponse>(result);
                if (formattedFcmResponse.success == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} {ex.StackTrace}");
                return false;
            }
        }
    }

    public class FCMServerResponse
    {
        public int failure { get; set; }
        public int success { get; set; }
        public string multicast_id { get; set; }
    }
}
