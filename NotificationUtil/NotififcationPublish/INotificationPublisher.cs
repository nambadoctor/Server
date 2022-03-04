using DataModel.Mongo.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationUtil.NotificationPublish
{
    public interface INotificationPublisher
    {
        public Task<bool> BuildAndPublishNotifications(EventQueue eventQueue);
    }
}
