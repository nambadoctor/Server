﻿using DataModel.Mongo;
using DataModel.Mongo.Notification;
using DataModel.Shared;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using Notification.Sms;
using NotificationUtil.Mode.SMS;

namespace NotificationUtil.NotificationPublish
{
    public class NotificationPublisher : INotificationPublisher
    {
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmentRepository;

        private readonly INotificationUserConfigurationRepository notificationUserConfigurationRepository;
        private readonly INotificationQueueRepository notificationQueueRepository;

        private ISmsBuilder smsBuilder;

        private ILogger logger;
        public NotificationPublisher(ISmsBuilder smsBuilder, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository, ILogger<NotificationPublisher> logger, INotificationQueueRepository notificationQueueRepository, INotificationUserConfigurationRepository notificationUserConfigurationRepository)
        {
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmentRepository = appointmentRepository;

            this.notificationQueueRepository = notificationQueueRepository;
            this.notificationUserConfigurationRepository = notificationUserConfigurationRepository;

            this.smsBuilder = smsBuilder;

            this.logger = logger;
        }
        public async Task<bool> BuildAndPublishNotifications(EventQueue eventQueue)
        {
            if (eventQueue.EventType == EventType.AppointmentBooked || eventQueue.EventType == EventType.AppointmentRescheduled || eventQueue.EventType == EventType.AppointmentCancelled)
            {
                var appointment = await appointmentRepository.GetAppointment(eventQueue.AppointmentId);
                if (appointment == null)
                {
                    logger.LogError($"NotificationPublisher: No appointment found with ID {eventQueue.AppointmentId}");
                    return false;
                }

                var subscriptions = await GetUserNotificationSubscriptionsForEvent(eventQueue, appointment.ServiceProviderId, appointment.OrganisationId);

                if (subscriptions.Item1.Count > 0)
                {
                    var queuedNotifications = await AddNotificationsToQueueForSubscriptions(subscriptions.Item1, subscriptions.Item2!, eventQueue);

                    logger.LogInformation($"Added {queuedNotifications.Count} notifications to QUEUE");

                    return true;
                }
                else
                {
                    logger.LogError($"NotificationPublisher: User not configured to send notification for appointment id: {eventQueue.AppointmentId}");
                    return false;
                }
            }
            else if (eventQueue.EventType == EventType.Referred)
            {
                var subscriptions = await GetUserNotificationSubscriptionsForEvent(eventQueue, eventQueue.ServiceProviderId, eventQueue.OrganisationId);

                if (subscriptions.Item1.Count > 0)
                {
                    var queuedNotifications = await AddNotificationsToQueueForSubscriptions(subscriptions.Item1, subscriptions.Item2!, eventQueue);

                    logger.LogInformation($"Added {queuedNotifications.Count} notifications to QUEUE");

                    return true;
                }
                else
                {
                    logger.LogError($"NotificationPublisher: User not configured to send notification for appointment id: {eventQueue.AppointmentId}");
                    return false;
                }
            }
            else if (eventQueue.EventType == EventType.Followup)
            {
                var subscriptions = await GetUserNotificationSubscriptionsForEvent(eventQueue, eventQueue.ServiceProviderId, eventQueue.OrganisationId);

                if (subscriptions.Item1.Count > 0)
                {
                    var queuedNotifications = await AddNotificationsToQueueForSubscriptions(subscriptions.Item1, subscriptions.Item2!, eventQueue);

                    logger.LogInformation($"Added {queuedNotifications.Count} notifications to QUEUE");

                    return true;
                }
                else
                {
                    logger.LogError($"NotificationPublisher: User not configured to send notification for appointment id: {eventQueue.AppointmentId}");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task<List<NotificationQueue>> AddNotificationsToQueueForSubscriptions(List<NotificationSubscription> subscriptions, NotificationUserConfiguration config, EventQueue eventQueue)
        {
            var notificationsNeedToBeQueued = await GetNotificationForSubscriptionList(subscriptions, config, eventQueue);

            //Delete first directive
            if (eventQueue.EventType == EventType.AppointmentCancelled || eventQueue.EventType == EventType.AppointmentRescheduled)
            {
                await notificationQueueRepository.RemoveAllMatchingIdList(eventQueue.AppointmentId);
                logger.LogInformation($"Removed notification events for appointment id: {eventQueue.AppointmentId}");
            }

            await notificationQueueRepository.AddMany(notificationsNeedToBeQueued);

            return notificationsNeedToBeQueued;
        }

        /// <summary>
        /// Returns ToSendToSelf, ToSendToCustomers
        /// </summary>
        /// <param name="eventQueue"></param>
        /// <param name="serviceProviderId"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        private async Task<(List<NotificationSubscription>, NotificationUserConfiguration?)> GetUserNotificationSubscriptionsForEvent(EventQueue eventQueue, string serviceProviderId, string organisationId)
        {
            var subscriptions = new List<NotificationSubscription>();

            var userConfiguration = await notificationUserConfigurationRepository.GetByServiceProvider(serviceProviderId, organisationId);

            logger.LogInformation($"Received userConfiguration for userid {serviceProviderId}");

            if (userConfiguration == null || userConfiguration.SubscribedNotifications == null)
            {
                logger.LogInformation($"No Notification subscription exists for service provider:{serviceProviderId} organisation: {organisationId}");
                return (subscriptions, null);
            }
            else
            {
                if (userConfiguration.SubscribedNotifications.Count > 0)
                {
                    foreach (var sub in userConfiguration.SubscribedNotifications)
                    {
                        if (IsSubscriptionOfEventType(eventQueue, sub.SubscriptionType))
                        {
                            subscriptions.Add(sub);
                        }
                    }
                }
            }

            logger.LogInformation($"Found {subscriptions.Count} subscriptions for Event {eventQueue.EventType}");

            return (subscriptions, userConfiguration);
        }

        private bool IsSubscriptionOfEventType(EventQueue eventQueue, SubscriptionType subscriptionType)
        {
            var isMatch = false;

            if (eventQueue.EventType == EventType.AppointmentBooked || eventQueue.EventType == EventType.AppointmentCancelled || eventQueue.EventType == EventType.AppointmentRescheduled)
            {
                if (subscriptionType == SubscriptionType.AppointmentReminder || subscriptionType == SubscriptionType.AppointmentStatus)
                {
                    isMatch = true;
                }
            }
            else if (eventQueue.EventType == EventType.Referred && subscriptionType == SubscriptionType.Referral)
            {
                isMatch = true;
            }

            else if (eventQueue.EventType == EventType.Followup && subscriptionType == SubscriptionType.Followup)
            {
                isMatch = true;
            }

            return isMatch;
        }

        private async Task<List<NotificationQueue>> GetNotificationForSubscriptionList(List<NotificationSubscription> subscriptionList, NotificationUserConfiguration config, EventQueue eventQueue)
        {
            var notifications = new List<NotificationQueue>();

            if (eventQueue.EventType == EventType.AppointmentBooked || eventQueue.EventType == EventType.AppointmentCancelled || eventQueue.EventType == EventType.AppointmentRescheduled)
            {
                var appointmentData = await GetAppointmentData(eventQueue.AppointmentId);

                foreach (var sub in subscriptionList)
                {
                    notifications.AddRange(GetAppointmentNotificationsForSubscription(sub, appointmentData.Item1, appointmentData.Item2, appointmentData.Item3, config.OrganisationName));
                }
            }
            else if (eventQueue.EventType == EventType.Referred)
            {
                var customer = await customerRepository.GetCustomerProfile(eventQueue.CustomerId, eventQueue.OrganisationId);
                var sp = await serviceProviderRepository.GetServiceProviderProfile(eventQueue.ServiceProviderId, eventQueue.OrganisationId);
                var custPhoneNumber = customer.PhoneNumbers.First();
                foreach (var sub in subscriptionList)
                {
                    if (sub.SubscriptionType == SubscriptionType.Referral)
                        notifications.AddRange(
                        GetReferralNotificationsForSubscription(
                            sub,
                            customer.FirstName + " " + customer.LastName,
                            custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number,
                            "Dr. " + sp.FirstName + " " + sp.LastName,
                            config.OrganisationName,
                            eventQueue.RecieverNumber,
                            eventQueue.CustomMessage
                        )
                    );
                }
            }
            else if (eventQueue.EventType == EventType.Followup)
            {
                var customer = await customerRepository.GetCustomerProfile(eventQueue.CustomerId, eventQueue.OrganisationId);
                var sp = await serviceProviderRepository.GetServiceProviderProfile(eventQueue.ServiceProviderId, eventQueue.OrganisationId);
                var custPhoneNumber = customer.PhoneNumbers.First();
                var spPhoneNumber = sp.PhoneNumbers.First();
                foreach (var sub in subscriptionList)
                {
                    if (sub.SubscriptionType == SubscriptionType.Followup)
                        notifications.AddRange(
                        GetFollowupNotificationsForSubscription(
                            sub,
                            customer.FirstName + " " + customer.LastName,
                            custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number,
                            "Dr. " + sp.FirstName + " " + sp.LastName,
                            spPhoneNumber.CountryCode.Replace("+", "") + spPhoneNumber.Number,
                            config.OrganisationName,
                            eventQueue.CustomMessage,
                            eventQueue.ScheduledDateTime
                        )
                    );
                }
            }

            return notifications;
        }

        private List<NotificationQueue> GetReferralNotificationsForSubscription(NotificationSubscription notificationSubscription, string custName, string custPhoneNumber, string spName, string organisationName, string recieverPhone, string reason)
        {

            var notifications = new List<NotificationQueue>();

            if (notificationSubscription.IsEnabledForSelf)
            {
                notifications.Add(smsBuilder.GetReferralSms(recieverPhone, custName, custPhoneNumber, spName, organisationName, reason, DateTime.UtcNow));
            }
            if (notificationSubscription.IsEnabledForCustomers)
            {
                //todo new template if needed
            }

            return notifications;
        }

        private List<NotificationQueue> GetFollowupNotificationsForSubscription(NotificationSubscription notificationSubscription, string custName, string custPhoneNumber, string spName, string spPhoneNumber, string organisationName, string reason, DateTime scheduledDate)
        {

            var notifications = new List<NotificationQueue>();

            if (notificationSubscription.IsEnabledForSelf)
            {
                notifications.Add(smsBuilder.GetFollowupSms(spPhoneNumber, custName, custPhoneNumber, organisationName, reason, scheduledDate));
            }
            if (notificationSubscription.IsEnabledForCustomers)
            {
                notifications.Add(smsBuilder.GetFollowupSms(custPhoneNumber, spName, spPhoneNumber, organisationName, reason, scheduledDate));
            }

            return notifications;
        }

        private List<NotificationQueue> GetAppointmentNotificationsForSubscription(NotificationSubscription notificationSubscription, Appointment appointment, string custPhoneNumber, string spPhoneNumber, string organisationName)
        {

            var notifications = new List<NotificationQueue>();

            if (notificationSubscription.SubscriptionType == SubscriptionType.AppointmentStatus)
            {
                if (notificationSubscription.IsEnabledForSelf)
                {
                    if (appointment.ScheduledAppointmentStartTime > DateTime.UtcNow)
                    {
                        notifications.Add(smsBuilder.GetFutureAppointmentStatusSMS(spPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.CustomerName, appointment.Status.ToString(), DateTime.UtcNow, appointment.AppointmentId.ToString(), organisationName));
                    }
                    else
                    {
                        notifications.Add(smsBuilder.GetAppointmentStatusSMS(spPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.CustomerName, appointment.Status.ToString(), DateTime.UtcNow, appointment.AppointmentId.ToString(), organisationName));
                    }
                }
                if (notificationSubscription.IsEnabledForCustomers)
                {
                    if (appointment.ScheduledAppointmentStartTime > DateTime.UtcNow)
                    {
                        notifications.Add(smsBuilder.GetFutureAppointmentStatusSMS(custPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.ServiceProviderName, appointment.Status.ToString(), DateTime.UtcNow, appointment.AppointmentId.ToString(), organisationName));
                    }
                    else
                    {
                        notifications.Add(smsBuilder.GetAppointmentStatusSMS(custPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.ServiceProviderName, appointment.Status.ToString(), DateTime.UtcNow, appointment.AppointmentId.ToString(), organisationName));
                    }
                }

            }

            if (notificationSubscription.SubscriptionType == SubscriptionType.AppointmentReminder && appointment.Status != AppointmentStatus.Cancelled)
            {
                foreach (var interval in notificationSubscription.MinuteIntervals)
                {
                    var toBeNotifiedTime = appointment.ScheduledAppointmentStartTime!.Value.AddMinutes(-interval);

                    if (toBeNotifiedTime > DateTime.UtcNow)
                    {
                        if (notificationSubscription.IsEnabledForSelf)
                        {
                            notifications.Add(smsBuilder.GetAppointmentReminderSMS(spPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.CustomerName, toBeNotifiedTime, appointment.AppointmentId.ToString(), organisationName));
                        }
                        if (notificationSubscription.IsEnabledForCustomers)
                        {
                            notifications.Add(smsBuilder.GetAppointmentReminderSMS(custPhoneNumber, appointment.ScheduledAppointmentStartTime!.Value, appointment.ServiceProviderName, toBeNotifiedTime, appointment.AppointmentId.ToString(), organisationName));
                        }
                    }
                    else
                    {
                        logger.LogInformation($"Too less time to fire reminder. Scheduled notification time:{toBeNotifiedTime} CALCULATED TIME LEFT TO NOTIFY:{interval} Current time:{DateTime.UtcNow}");
                    }

                }

            }

            return notifications;
        }

        private async Task<(Appointment, string, string)> GetAppointmentData(string appointmentId)
        {
            var appointment = await appointmentRepository.GetAppointment(appointmentId);

            if (appointment == null)
                throw new FileNotFoundException($"Not found Appointment with id:{appointmentId}");

            var customerProfile = await customerRepository.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

            if (customerProfile == null)
                throw new FileNotFoundException($"Not found Customer with id:{appointment.CustomerId}");

            var spProfile = await serviceProviderRepository.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

            if (spProfile == null)
                throw new FileNotFoundException($"Not found ServiceProvider with id:{appointment.ServiceProviderId}");

            var custPhoneNumber = customerProfile.PhoneNumbers.First();
            var spPhoneNumber = spProfile.PhoneNumbers.First();

            if (custPhoneNumber == null)
            {
                throw new FileNotFoundException($"Not found custPhoneNumber for {customerProfile.CustomerId}");
            }
            if (spPhoneNumber == null)
            {
                throw new FileNotFoundException($"Not found spPhoneNumber for {spProfile.ServiceProviderId}");
            }

            return (appointment, custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number, spPhoneNumber.CountryCode.Replace("+", "") + spPhoneNumber.Number);
        }
    }
}
