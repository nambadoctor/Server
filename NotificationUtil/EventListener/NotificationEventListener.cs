using DataModel.Mongo.Notification;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.EventListener
{
    public class NotificationEventListener : INotificationEventListener
    {
        private readonly INotificationQueueRepository notificationQueueRepository;
        private readonly ILogger<NotificationEventListener> logger;
        public NotificationEventListener(INotificationQueueRepository notificationQueueRepository, ILogger<NotificationEventListener> logger)
        {
            this.notificationQueueRepository = notificationQueueRepository;
            this.logger = logger;
        }

        public async Task TriggerEvent(string appointmentId, EventType eventType)
        {
            logger.LogInformation($"Started {eventType} TRIGGERED FOR APPOINTMENT: {appointmentId}");

            try
            {
                var newEvent = EventUtility.GetQueueObject(appointmentId, eventType);

                var result = await EventUtility.TriggerEvent(newEvent);

                logger.LogInformation($"Finished {eventType} TRIGGERED FOR APPOINTMENT: {appointmentId} WITH STATUS:{result}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error {eventType} TRIGGERED FOR APPOINTMENT: {appointmentId} WITH ERROR:{ex.Message} {ex.StackTrace}");
            }
        }

    }
}
