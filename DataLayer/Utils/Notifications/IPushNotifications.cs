using DataModel.Mongo;

using System.Threading.Tasks;

namespace DataLayer.Notifications
{
    public interface IPushNotifications
    {
        public Task<bool> SendNotificationAsync(string userId, string title, string body, string type, string id, NotificationInfo notificationInfo);
    }
}

