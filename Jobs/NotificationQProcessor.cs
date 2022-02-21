using DataModel.Mongo.Notification;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using NotificationUtil.Trigger;
using System;
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

            var pendingQueue = await notificationQueueRepository.GetPending();

            foreach (var queue in pendingQueue)
            {
                await notificationQueueRepository.Remove(queue.NotificationQueueId.ToString());
                FireNotification(queue);
            }

            logger.LogInformation($"NotificationQProcessor function finished execution at: {DateTime.Now}");
        }

        private void FireNotification(NotificationQueue notificationQueue)
        {
            //Fire appointment status notif
            if (notificationQueue.NotificationType == NotificationType.Cancellation ||
                notificationQueue.NotificationType == NotificationType.ImmediateConfirmation ||
                notificationQueue.NotificationType == NotificationType.Reschedule)
            {
                notificationBroadcast.FireAppointmentStatusNotification(notificationQueue.AppointmentId);
            }
            else if (notificationQueue.NotificationType == NotificationType.Reminder)
            {
                notificationBroadcast.FireReminderNotification(notificationQueue.AppointmentId);
            }

        }
    }
}
