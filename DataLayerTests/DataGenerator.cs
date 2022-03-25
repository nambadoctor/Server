using DataModel.Mongo.Notification;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Mongo.Configuration;

namespace LayerTests
{
    public static class DataGenerator
    {

        const string globalDrId = "61f395acee2b9622eaad5303";
        const string globalOrgId = "61f3957eee2b9622eaad52fe";
        public static NotificationUserConfiguration GenerateNotificationConfigForGlobalUser()
        {

            NotificationUserConfiguration notificationUserConfiguration = new NotificationUserConfiguration();

            notificationUserConfiguration.UserConfigurationId = ObjectId.GenerateNewId();
            notificationUserConfiguration.ServiceProviderId = globalDrId;
            notificationUserConfiguration.OrganisationId = globalOrgId;
            notificationUserConfiguration.OrganisationName = "GlobalOrg";
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
                    IsEnabledForCustomers = true,
                    IsEnabledForSelf = true,
                    SubscriptionType = SubscriptionType.AppointmentStatus
                }
            );

            subscriptions.Add(
                new NotificationSubscription
                {
                    IsEnabledForCustomers = false,
                    IsEnabledForSelf = true,
                    SubscriptionType = SubscriptionType.Referral
                }
            );

            return subscriptions;
        }

        public static SettingsConfiguration GetSampleSettingsConfiguration()
        {
            var settings = new SettingsConfiguration();
            settings.OrganisationId = globalOrgId;
            settings.ServiceProviderId = globalDrId;
            settings.AppointmentSettings = new AppointmentSettings();
            settings.AppointmentSettings.AppointmentReasons = new List<string> { "Fever", "Pregnancy", "STDs" };

            return settings;
        }
    }
}
