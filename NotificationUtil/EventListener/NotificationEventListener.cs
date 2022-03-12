﻿using DataModel.Mongo.Notification;
using Microsoft.Extensions.Logging;
using NotificationUtil.NotificationPublish;

namespace NotificationUtil.EventListener
{
    public class NotificationEventListener : INotificationEventListener
    {
        private readonly ILogger<NotificationEventListener> logger;
        private readonly INotificationPublisher notificationPublisher;
        public NotificationEventListener(ILogger<NotificationEventListener> logger, INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;
            this.logger = logger;
        }

        public async Task TriggerEvent(string appointmentId, EventType eventType)
        {
            logger.LogInformation($"Started {eventType} Publish notification for appointment: {appointmentId}");

            try
            {
                var newEvent = EventUtility.GetQueueObject(appointmentId, eventType);

                var isPublished = await notificationPublisher.BuildAndPublishNotifications(newEvent);

                logger.LogInformation($"Finished {eventType} Publish notification for appointment: {appointmentId} WITH STATUS:{isPublished}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error {eventType} Publish notification for appointment: {appointmentId} WITH ERROR:{ex.Message} {ex.StackTrace}");
            }
        }

    }
}
