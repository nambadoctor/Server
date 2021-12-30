using DataModel.Mongo;
using DataModel.Shared;
using System;
using System.Threading.Tasks;

namespace DataLayer.Notifications
{
    public class PushNotifications : IPushNotifications
    {

        public PushNotifications()
        {
        }

        public async Task<bool> SendNotificationAsync(string userId, string title, string body, string type, string id, NotificationInfo notificationInfo)
        {
            try
            {
                if (notificationInfo != null)
                {
                    if (notificationInfo.DeviceType == "apn")
                    {
                        var appleNotification = new ApplePushNotification();
                        var appleNotificationResult = await appleNotification.SendApnNotificationAsync(notificationInfo.DeviceToken, title, body, type, id);
                        return appleNotificationResult;
                    }
                    else
                    {
                        var fcmNotification = new FirebasePushNotification();
                        var fcmNotificationResult = await fcmNotification.SendNotification(notificationInfo.DeviceToken, title, body, id, type);
                        return fcmNotificationResult;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

