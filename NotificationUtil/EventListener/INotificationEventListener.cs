using DataModel.Mongo.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.EventListener
{
    public interface INotificationEventListener
    {
        public Task TriggerAppointmentEvent(string appointmentId, EventType eventType);
        public Task TriggerReferEvent(string customerId, string serviceProviderId, string organisationId, string phoneNumber, string reason, EventType eventType);

    }
}
