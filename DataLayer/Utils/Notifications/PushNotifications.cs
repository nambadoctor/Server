using DataModel.Mongo;
using DataModel.Shared;
using System;
using System.Threading.Tasks;

namespace DataLayer.Notifications
{
    public class PushNotifications : IPushNotifications
    {
        INDLogger _NDLogger;

        public PushNotifications(INDLogger NDLogger)
        {
            _NDLogger = NDLogger;
        }

        public async Task<bool> SendNotificationAsync(string userId, string title, string body, string type, string id, NotificationInfo notificationInfo)
        {
            try
            {
                if (notificationInfo != null)
                {
                    if (notificationInfo.DeviceType == "apn")
                    {
                        _NDLogger.LogEvent("SENDING APN NOTIFICATION");
                        var appleNotification = new ApplePushNotification(_NDLogger);
                        var appleNotificationResult = await appleNotification.SendApnNotificationAsync(notificationInfo.DeviceToken, title, body, type, id);
                        _NDLogger.LogEvent($"SENDING APN NOTIFICATION {appleNotificationResult}");
                        _NDLogger.LogEvent("End Successfully SendNotification");
                        return appleNotificationResult;
                    }
                    else
                    {
                        _NDLogger.LogEvent("SENDING FCM NOTIFICATION");
                        var fcmNotification = new FirebasePushNotification();
                        var fcmNotificationResult = await fcmNotification.SendNotification(notificationInfo.DeviceToken, title, body, id, type);
                        _NDLogger.LogEvent($"SENDING FCM NOTIFICATION {fcmNotificationResult}");
                        _NDLogger.LogEvent("End Successfully SendNotification");
                        return fcmNotificationResult;
                    }
                }
                else
                {
                    _NDLogger.LogEvent("End Failure SendNotification");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _NDLogger.LogEvent($"End Failure SendNotification:{ex}");
                return false;
            }
        }
    }
}

