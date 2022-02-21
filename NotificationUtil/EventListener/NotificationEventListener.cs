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
        private readonly ILogger<NotificationEventListener> _logger;
        public NotificationEventListener(INotificationQueueRepository notificationQueueRepository, ILogger<NotificationEventListener> logger)
        {
            this.notificationQueueRepository = notificationQueueRepository;
            this._logger = logger;
        }

        public async Task RescheduleAppointmentEvent(string appointmentId, DateTime appointmentTime)
        {
            await notificationQueueRepository.RemoveAllMatchingId(appointmentId);

            await AddNewAppointmentEventToQueue(appointmentId, appointmentTime);
        }

        public async Task CancelAppointmentEvent(string appointmentId)
        {
            var cancelNotificationQueue = EventUtility.GetQueueObject(appointmentId, NotificationType.Cancellation, DateTime.UtcNow);

            await notificationQueueRepository.RemoveAllMatchingId(appointmentId);

            await notificationQueueRepository.Add(cancelNotificationQueue);
        }

        public async Task NewAppointmentEvent(string appointmentId, DateTime appointmentTime)
        {
            await AddNewAppointmentEventToQueue(appointmentId, appointmentTime);
        }

        private async Task AddNewAppointmentEventToQueue(string appointmentId, DateTime appointmentTime)
        {
            var immediateNotificationQueue = EventUtility.GetQueueObject(appointmentId, NotificationType.ImmediateConfirmation, appointmentTime);

            var twentyFourHourNQ = immediateNotificationQueue;
            twentyFourHourNQ.NotificationType = NotificationType.Reminder;
            twentyFourHourNQ.NotificationScheduledTime = appointmentTime.AddDays(1);

            var twelveHourNQ = immediateNotificationQueue;
            twentyFourHourNQ.NotificationType = NotificationType.Reminder;
            twentyFourHourNQ.NotificationScheduledTime = appointmentTime.AddHours(12);

            await notificationQueueRepository.Add(immediateNotificationQueue);
            await notificationQueueRepository.Add(twentyFourHourNQ);
            await notificationQueueRepository.Add(twelveHourNQ);
        }
    }
}
