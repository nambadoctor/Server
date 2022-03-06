using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DataModel.Mongo.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.GenericRepository.Interfaces;
using Newtonsoft.Json;
using NotificationUtil.NotificationPublish;

namespace Jobs
{
    public class NotificationQPublisher
    {
        private readonly ILogger<NotificationQPublisher> logger;
        private readonly INotificationPublisher notificationPublisher;

        public NotificationQPublisher(ILogger<NotificationQPublisher> log, INotificationPublisher notificationPublisher)
        {
            logger = log;
            this.notificationPublisher = notificationPublisher;
        }

        [FunctionName("NotificationQPublisher")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EventQueue))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                EventQueue eventBody = JsonConvert.DeserializeObject<EventQueue>(requestBody);

                var result = await notificationPublisher.BuildAndPublishNotifications(eventBody);

                logger.LogInformation($"NotificationQPublisher processed a request with status:{result}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error publishing notification: {ex.Message} {ex.StackTrace}");
            }

            return new OkObjectResult("Event successfully logged");
        }

    }
}

