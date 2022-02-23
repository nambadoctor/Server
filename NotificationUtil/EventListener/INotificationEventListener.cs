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
        public Task NewAppointmentEvent(string appointmentId, DateTime? appointmentTime);
        public Task CancelAppointmentEvent(string appointmentId);
        public Task RescheduleAppointmentEvent(string appointmentId, DateTime? appointmentTime);

    }
}
