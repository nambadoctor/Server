using DataModel.Mongo;
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
        private IOrganisationRepository organisationRepository;

        private readonly INotificationUserConfigurationRepository notificationUserConfigurationRepository;
        private readonly INotificationQueueRepository notificationQueueRepository;

        private ISmsBuilder smsBuilder;

        private ILogger logger;
        public NotificationPublisher(ISmsBuilder smsBuilder, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository, ILogger<NotificationPublisher> logger, INotificationQueueRepository notificationQueueRepository, INotificationUserConfigurationRepository notificationUserConfigurationRepository, IOrganisationRepository organisationRepository)
        {
            this.organisationRepository = organisationRepository;
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

                if (subscriptions.Count > 0)
                {
                    var queuedNotifications = await AddNotificationsToQueueForSubscriptions(subscriptions, eventQueue);

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

                if (subscriptions.Count > 0)
                {
                    var queuedNotifications = await AddNotificationsToQueueForSubscriptions(subscriptions, eventQueue);

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

        private async Task<List<NotificationQueue>> AddNotificationsToQueueForSubscriptions(List<NotificationSubscription> subscriptions, EventQueue eventQueue)
        {
            var notificationsNeedToBeQueued = await GetNotificationForSubscriptionList(subscriptions, eventQueue);

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
        private async Task<List<NotificationSubscription>> GetUserNotificationSubscriptionsForEvent(EventQueue eventQueue, string serviceProviderId, string organisationId)
        {
            var subscriptions = new List<NotificationSubscription>();

            var userConfigurations = await notificationUserConfigurationRepository.GetByServiceProvider(serviceProviderId, organisationId);

            logger.LogInformation($"Received {userConfigurations.Count} userConfigurations for userid {serviceProviderId}");

            if (userConfigurations == null || userConfigurations.Count == 0)
            {
                logger.LogInformation($"No Notification configuration exists for service provider:{serviceProviderId} organisation: {organisationId}");
            }
            else
            {
                foreach (var configuration in userConfigurations)
                {
                    if (configuration.SubscribedNotifications != null && configuration.SubscribedNotifications.Count > 0)
                    {
                        foreach (var sub in configuration.SubscribedNotifications)
                        {
                            if (IsSubscriptionOfEventType(eventQueue, sub.SubscriptionType))
                            {
                                subscriptions.Add(sub);
                            }
                        }
                    }
                }
            }

            logger.LogInformation($"Found {subscriptions.Count} subscriptions for Event {eventQueue.EventType}");

            return subscriptions;
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

            return isMatch;
        }

        private async Task<List<NotificationQueue>> GetNotificationForSubscriptionList(List<NotificationSubscription> subscriptionList, EventQueue eventQueue)
        {
            var notifications = new List<NotificationQueue>();

            if (eventQueue.EventType == EventType.AppointmentBooked || eventQueue.EventType == EventType.AppointmentCancelled || eventQueue.EventType == EventType.AppointmentRescheduled)
            {
                var appointmentData = await GetAppointmentData(eventQueue.AppointmentId);

                foreach (var sub in subscriptionList)
                {
                    notifications.AddRange(GetAppointmentNotificationsForSubscription(sub, appointmentData.Item1, appointmentData.Item2, appointmentData.Item3, appointmentData.Item4));
                }
            }
            else if (eventQueue.EventType == EventType.Referred)
            {
                var customer = await customerRepository.GetCustomerProfile(eventQueue.CustomerId, eventQueue.OrganisationId);
                var sp = await serviceProviderRepository.GetServiceProviderProfile(eventQueue.ServiceProviderId, eventQueue.OrganisationId);
                var custPhoneNumber = customer.PhoneNumbers.First();
                var organisation = await organisationRepository.GetById(eventQueue.OrganisationId);
                foreach (var sub in subscriptionList)
                {
                    notifications.AddRange(
                        GetReferralNotificationsForSubscription(
                            sub,
                            customer.FirstName + " " + customer.LastName,
                            custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number,
                            "Dr. " + sp.FirstName + " " + sp.LastName,
                            organisation.Name,
                            eventQueue.RecieverNumber,
                            eventQueue.CustomMessage
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

        private async Task<(Appointment, string, string, string)> GetAppointmentData(string appointmentId)
        {
            var appointment = await appointmentRepository.GetAppointment(appointmentId);

            if (appointment == null)
                throw new FileNotFoundException($"Not found Appointment with id:{appointmentId}");

            var chosenOrg = await organisationRepository.GetById(appointment.OrganisationId);

            if (chosenOrg == null)
                throw new FileNotFoundException($"Not found Organisation with id:{appointment.OrganisationId}");

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

            return (appointment, custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number, spPhoneNumber.CountryCode.Replace("+", "") + spPhoneNumber.Number, chosenOrg.Name);
        }
    }
}
