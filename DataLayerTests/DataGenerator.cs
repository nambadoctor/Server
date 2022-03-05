using DataModel.Mongo.Notification;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerTests
{
    public static class DataGenerator
    {
        public static NotificationUserConfiguration GenerateNotificationConfigForGlobalUser()
        {
            var globalDrId = "61f395acee2b9622eaad5303";
            var globalOrgId = "61f3957eee2b9622eaad52fe";

            NotificationUserConfiguration notificationUserConfiguration = new NotificationUserConfiguration();

            notificationUserConfiguration.UserConfigurationId = ObjectId.GenerateNewId();
            notificationUserConfiguration.ServiceProviderId = globalDrId;
            notificationUserConfiguration.OrganisationId = globalOrgId;
            notificationUserConfiguration.Name = "Global doctor";
            notificationUserConfiguration.CreatedDateTime = DateTime.UtcNow;

            notificationUserConfiguration.SubscribedNotifications = GetSampleNotificationSubscriptions();

            return notificationUserConfiguration;
        }

        public static List<NotificationSubscription> GetSampleNotificationSubscriptions()
        {
            var subscriptions = new List<NotificationSubscription>();

            subscriptions.Add(
                new NotificationSubscription
                {
                    IsEnabledForCustomers = true,
                    IsEnabledForSelf = true,
                    MinuteIntervals = new List<int> { 120, 10 },
                    SubscriptionType = SubscriptionType.AppointmentReminder
                }
            );

            subscriptions.Add(
                new NotificationSubscription
                {
                    IsEnabledForCustomers = false,
                    IsEnabledForSelf = true,
                    SubscriptionType = SubscriptionType.AppointmentStatus
                }
            );

            return subscriptions;
        }
    }
}
