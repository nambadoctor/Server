using DataModel.Mongo.Notification;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.EventListener
{
    public static class EventUtility
    {
        public static EventQueue GetAppointmentQueueObject(string appointmentId, EventType eventType)
        {
            var notificationQueue = new EventQueue();

            notificationQueue.AppointmentId = appointmentId;

            notificationQueue.EventType = eventType;

            notificationQueue.CreatedDateTime = DateTime.UtcNow;

            return notificationQueue;
        }

        public static EventQueue GetReferQueueObject(string customerId, string serviceProviderId, string recieverPhone, string customMessage, string OrganisationId, EventType eventType)
        {
            var notificationQueue = new EventQueue();

            notificationQueue.CustomerId = customerId;

            notificationQueue.ServiceProviderId = serviceProviderId;

            notificationQueue.OrganisationId = OrganisationId;

            notificationQueue.RecieverNumber = recieverPhone;

            notificationQueue.EventType = eventType;

            notificationQueue.CustomMessage = customMessage;

            notificationQueue.CreatedDateTime = DateTime.UtcNow;

            return notificationQueue;
        }
    }
}
