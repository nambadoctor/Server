using DataModel.Mongo.Notification;
using DataModel.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using Newtonsoft.Json;
using NotificationUtil.Trigger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jobs
{
    public class NotificationQProcessor
    {
        private readonly INotificationQueueRepository notificationQueueRepository;
        private readonly INotificationBroadcast notificationBroadcast;
        public NotificationQProcessor(INotificationQueueRepository notificationQueueRepository, INotificationBroadcast notificationBroadcast)
        {
            this.notificationQueueRepository = notificationQueueRepository;
            this.notificationBroadcast = notificationBroadcast;
        }

        [FunctionName("NotificationQProcessor")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"NotificationQProcessor function executed at: {DateTime.Now}");

            try
            {
                var pendingQueue = await notificationQueueRepository.GetPending();

                logger.LogInformation($"Pending queue count: {pendingQueue.Count}");

                foreach (var queue in pendingQueue)
                {
                    await notificationQueueRepository.Remove(queue.NotificationQueueId.ToString());

                    logger.LogInformation($"Removed event: {queue.NotificationQueueId} {queue.AppointmentId} {queue.NotificationType}");

                    await FireNotification(queue, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"NotificationQProcessor function unhandled exception: {ex.Message} {ex.StackTrace}");
            }

            logger.LogInformation($"NotificationQProcessor function finished execution at: {DateTime.Now}");
        }

        private async Task FireNotification(NotificationQueue notificationQueue, ILogger logger)
        {
            //Fire appointment status notif
            if (notificationQueue.NotificationType == NotificationType.Cancellation ||
                notificationQueue.NotificationType == NotificationType.ImmediateConfirmation ||
                notificationQueue.NotificationType == NotificationType.Reschedule)
            {
                await notificationBroadcast.FireAppointmentStatusNotification(notificationQueue.AppointmentId);
            }
            else if (notificationQueue.NotificationType == NotificationType.Reminder)
            {
                await notificationBroadcast.FireReminderNotification(notificationQueue.AppointmentId);
            }

            logger.LogInformation($"Fired notification event: {notificationQueue.NotificationQueueId} {notificationQueue.AppointmentId} {notificationQueue.NotificationType}");

        }
    }
}
