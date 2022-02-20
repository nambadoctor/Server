using DataModel.Shared;
using Jobs.Models;
using Jobs.Repository;
using Jobs.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Jobs
{
    public class NotificationQPublisher
    {
        private readonly ILogger<NotificationQPublisher> _logger;
        private readonly INotificationQueueRepository notificationQueueRepository;

        public NotificationQPublisher(ILogger<NotificationQPublisher> logger, INotificationQueueRepository notificationQueueRepository)
        {
            _logger = logger;
            this.notificationQueueRepository = notificationQueueRepository;
        }

        [FunctionName("NotificationQPublisher")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Queue/Dequeue Notification" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "AppointmentId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The AppointmentId")]
        [OpenApiParameter(name: "NotificationType", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The NotificationTypes: {TwentyFourHourReminder,TwelveHourReminder,ImmediateConfirmation,Cancellation}")]
        [OpenApiParameter(name: "ScheduledTime", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The appointment time")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The ERROR response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string appointmentId = req.Query["AppointmentId"];
            string notificationType = req.Query["NotificationType"];
            string scheduledTime = req.Query["ScheduledTime"];

            if (string.IsNullOrWhiteSpace(appointmentId) || string.IsNullOrWhiteSpace(notificationType) || string.IsNullOrWhiteSpace(scheduledTime))
            {
                _logger.LogError("Bad request at NotificationQPublisher");
                return new BadRequestResult();
            }

            var notificationObj = ParseRequest(appointmentId, notificationType, scheduledTime);

            _logger.LogInformation($"Parsed request successfully, Queue Id:  {notificationObj.NotificationQueueId}");

            await ProcessRequest(notificationObj);

            _logger.LogInformation("Queue processed successfully");

            return new OkObjectResult("Success");
        }

        private async Task ProcessRequest(NotificationQueue notificationQueue)
        {
            if (notificationQueue.NotificationType == NotificationType.Cancellation)
            {
                await notificationQueueRepository.RemoveAllMatchingId(notificationQueue.AppointmentId);
            }
            else if (notificationQueue.NotificationType == NotificationType.Reschedule)
            {
                await notificationQueueRepository.RemoveAllMatchingId(notificationQueue.AppointmentId);

                var twentyFourHourNQ = notificationQueue;
                twentyFourHourNQ.NotificationType = NotificationType.TwentyFourHourReminder;

                var twelveHourNQ = notificationQueue;
                twelveHourNQ.NotificationType = NotificationType.TwelveHourReminder;

                var immediateNQ = notificationQueue;
                immediateNQ.NotificationType = NotificationType.ImmediateConfirmation;

                await notificationQueueRepository.Add(twentyFourHourNQ);
                await notificationQueueRepository.Add(twelveHourNQ);
                await notificationQueueRepository.Add(immediateNQ);
            }
            else
            {
                await notificationQueueRepository.Add(notificationQueue);
            }
        }

        private NotificationQueue ParseRequest(string appointmentId, string notificationType, string appointmentTime)
        {
            var notificationQueue = new NotificationQueue();

            notificationQueue.AppointmentId = appointmentId;

            Enum.TryParse(notificationType, out NotificationType notificationTypeEnum);
            notificationQueue.NotificationType = notificationTypeEnum;

            Double.TryParse(appointmentTime, out Double appointmentTimestamp);

            notificationQueue.NotificationScheduledTime = UtilityFunctions.UnixTimeStampToDateTime(appointmentTimestamp);

            notificationQueue.CreatedDateTime = DateTime.UtcNow;

            notificationQueue.NotificationQueueId = ObjectId.GenerateNewId();

            return notificationQueue;
        }
    }
}

