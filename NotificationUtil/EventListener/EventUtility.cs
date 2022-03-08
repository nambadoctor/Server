using DataModel.Mongo.Notification;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.EventListener
{
    public static class EventUtility
    {
        public static EventQueue GetQueueObject(string appointmentId, EventType eventType)
        {
            var notificationQueue = new EventQueue();

            notificationQueue.AppointmentId = appointmentId;

            notificationQueue.EventType = eventType;

            notificationQueue.CreatedDateTime = DateTime.UtcNow;

            return notificationQueue;
        }

        public static async Task<bool> TriggerEvent(EventQueue eventQueue)
        {
            string BaseUrl = "https://nambajobs.azurewebsites.net/api";
            string FunctionApiKey = "fzUv2QCow3Hlx1O8d4mEgj2o9VuUw8j8QbNnBib4imB71fiazrTuvQ==";

            //TO Switch for prod deployment
            string ProdBaseUrl = "https://nambajobsprod.azurewebsites.net/api/";
            string ProdFunctionApiKey = "LhOJMtFnsQPw3pVYRX0DJocyHoCpfr8qwWiqyHtuRzn0z/izqBaqFA==";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);

            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + $"/NotificationQPublisher?code={FunctionApiKey}"))
            {
                var jsonData = JsonConvert.SerializeObject(eventQueue);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
