using DataModel.Mongo.Notification;
using Microsoft.Extensions.Logging;

namespace NotificationUtil.EventListener
{
    public class NotificationEventListener : INotificationEventListener
    {
        private readonly ILogger<NotificationEventListener> logger;
        public NotificationEventListener(ILogger<NotificationEventListener> logger)
        {
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
