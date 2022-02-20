using DataModel.Shared;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MiddleWare.Utils
{
    public class QueueNotificationHelper
    {
        private HttpClient httpClient;
        private ILogger logger;
        private string BaseUrl;
        public QueueNotificationHelper(ILogger logger)
        {
            httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            this.httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            this.logger = logger;
            BaseUrl = "https://google.com";//TODO Update URL
        }

        public async Task QueueNotification(string appointmentId, NotificationType notificationType, DateTime scheduledTime)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/NotificationQPublisher?AppointmentId={appointmentId}&NotificationType={notificationType}&ScheduledTime={ToUnixEpochDate(scheduledTime) * 1000}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                logger.LogInformation(value);
            }
        }

        public static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}
