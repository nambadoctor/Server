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

        public async Task RescheduleAppointmentEvent(string appointmentId, DateTime? appointmentTime)
        {
            await notificationQueueRepository.RemoveAllMatchingId(appointmentId);

            if (appointmentTime.HasValue)
                await AddNewAppointmentEventToQueue(appointmentId, appointmentTime.Value);
            else
                _logger.LogError($"Appointment with no scheduled time at RescheduleAppointmentEvent: {appointmentId}");
        }

        public async Task CancelAppointmentEvent(string appointmentId)
        {
            var cancelNotificationQueue = EventUtility.GetQueueObject(appointmentId, NotificationType.Cancellation, DateTime.UtcNow);

            await notificationQueueRepository.RemoveAllMatchingId(appointmentId);

            _logger.LogInformation($"CancelAppointmentEvent Removed previous events for {appointmentId}");

            await notificationQueueRepository.Add(cancelNotificationQueue);

            _logger.LogInformation($"CancelAppointmentEvent Added cancel event {appointmentId}");
        }

        public async Task NewAppointmentEvent(string appointmentId, DateTime? appointmentTime)
        {
            if (appointmentTime.HasValue)
                await AddNewAppointmentEventToQueue(appointmentId, appointmentTime.Value);
            else
                _logger.LogError($"Appointment with no scheduled time at NewAppointmentEvent: {appointmentId}");
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

            _logger.LogInformation($"AddNewAppointmentEventToQueue Constructed events for {appointmentId}");

            await notificationQueueRepository.Add(immediateNotificationQueue);

            _logger.LogInformation($"AddNewAppointmentEventToQueue Added immediate notification event for {appointmentId}");

            await notificationQueueRepository.Add(twentyFourHourNQ);

            _logger.LogInformation($"AddNewAppointmentEventToQueue Added 24 hour reminder event for {appointmentId}");

            await notificationQueueRepository.Add(twelveHourNQ);

            _logger.LogInformation($"AddNewAppointmentEventToQueue Added 12 hour reminder event for {appointmentId}");
        }
    }
}
