using DataModel.Mongo.Notification;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using NotificationUtil.Mode.SMS;
using System;
using System.Threading.Tasks;

namespace Jobs
{
    public class NotificationQProcessor
    {
        private readonly INotificationQueueRepository notificationQueueRepository;
        private readonly ISmsRepository smsRepository;
        public NotificationQProcessor(INotificationQueueRepository notificationQueueRepository, ISmsRepository smsRepository)
        {
            this.notificationQueueRepository = notificationQueueRepository;
            this.smsRepository = smsRepository;
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

                    logger.LogInformation($"Removed event: {queue.NotificationQueueId} {queue.NotificationType}");

                    FireNotification(queue, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"NotificationQProcessor function unhandled exception: {ex.Message} {ex.StackTrace}");
            }

            logger.LogInformation($"NotificationQProcessor function finished execution at: {DateTime.Now}");
        }

        private void FireNotification(NotificationQueue notificationQueue, ILogger logger)
        {
            //Fire appointment status notif
            var response = smsRepository.SendSms(notificationQueue.Message, notificationQueue.UserPhoneNumber, notificationQueue.SenderId);

            logger.LogInformation($"Fired notification event: {notificationQueue.NotificationQueueId} with status: {response}");

        }
    }
}
