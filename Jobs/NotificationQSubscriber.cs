using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jobs.Models;
using Jobs.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using DataModel.Shared;
using NotificationUtil.Trigger;
using System.Linq;

namespace Jobs
{
    public class NotificationQSubscriber
    {
        private readonly INotificationQueueRepository notificationQueueRepository;
        private readonly INotificationBroadcast notificationBroadcast;
        public NotificationQSubscriber(INotificationQueueRepository notificationQueueRepository, INotificationBroadcast notificationBroadcast)
        {
            this.notificationQueueRepository = notificationQueueRepository;
            this.notificationBroadcast = notificationBroadcast;
        }

        [FunctionName("NotificationQSubscriber")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"NotificationQSubscriber function executed at: {DateTime.Now}");

            var pendingQueue = await notificationQueueRepository.GetPending();

            foreach (var queue in pendingQueue)
            {
                FireNotification(queue);
            }

            await RemoveFromQueue(pendingQueue.Select(nq => nq.NotificationQueueId.ToString()).ToList());

            logger.LogInformation($"NotificationQSubscriber function finished execution at: {DateTime.Now}");
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
            else if (notificationQueue.NotificationType == NotificationType.TwentyFourHourReminder || notificationQueue.NotificationType == NotificationType.TwelveHourReminder)
            {
                notificationBroadcast.FireReminderNotification(notificationQueue.AppointmentId);
            }

        }

        private async Task RemoveFromQueue(List<string> notificationQueueIds)
        {
            await notificationQueueRepository.RemoveAllMatchingIdList(notificationQueueIds);
        }
    }
}
