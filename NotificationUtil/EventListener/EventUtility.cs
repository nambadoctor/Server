using DataModel.Mongo.Notification;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.EventListener
{
    public static class EventUtility
    {
        public static NotificationQueue GetQueueObject(string appointmentId, NotificationType notificationType, DateTime appointmentTime)
        {
            var notificationQueue = new NotificationQueue();

            notificationQueue.AppointmentId = appointmentId;

            notificationQueue.NotificationType = notificationType;

            notificationQueue.NotificationScheduledTime = appointmentTime;

            notificationQueue.CreatedDateTime = DateTime.UtcNow;

            notificationQueue.NotificationQueueId = ObjectId.GenerateNewId();

            return notificationQueue;
        }
    }
}
