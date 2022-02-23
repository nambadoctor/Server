using DataModel.Mongo;
using DataModel.Shared;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using Notification.Sms;
using NotificationUtil.Mode.SMS;

namespace NotificationUtil.Trigger
{
    public class NotificationBroadcast : INotificationBroadcast
    {
        private ISmsService smsService;
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmentRepository;
        private ILogger logger;
        public NotificationBroadcast(ISmsService smsService, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository, ILogger<NotificationBroadcast> logger)
        {
            this.smsService = smsService;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmentRepository = appointmentRepository;
            this.logger = logger;
        }

        public async Task FireAppointmentStatusNotification(string appointmentId)
        {

            var appointmentData = await GetAppointmentData(appointmentId);
            var appointment = appointmentData.Item1;
            var customerNumber = appointmentData.Item2;
            var spNumber = appointmentData.Item3;

            logger.LogInformation($"Started Firing notification to Customer no:{customerNumber} Service Provider Number:{spNumber}");

            if (!appointment.ScheduledAppointmentStartTime.HasValue)
            {
                logger.LogError($"Appointment with no scheduled time at FireAppointmentStatusNotification: {appointmentId}");
                return;
            }

            if (WhiteList.IsServiceproviderWhitelisted(appointment.ServiceProviderId))
            {
                SendAppointmentStatusSmsToServiceProvider(
                customerNumber,
                appointment.ScheduledAppointmentStartTime.Value.ToUniversalTime(),
                appointment.CustomerName,
                appointment.Status.ToString()
                );

                logger.LogInformation($"Fired AppointmentStatus notification to Service Provider Number:{spNumber}");
            }
            else
            {
                logger.LogInformation($"IsServiceproviderWhitelisted is false for Service Provider Id:{appointment.ServiceProviderId}");
            }

            if (WhiteList.IsCustomerWhitelistedByServiceprovider(appointment.ServiceProviderId))
            {
                SendAppointmentStatusSmsToCustomer(
                spNumber,
                appointment.ScheduledAppointmentStartTime.Value.ToUniversalTime(),
                "Dr. " + appointment.ServiceProviderName,
                appointment.Status.ToString()
                );

                logger.LogInformation($"Fired AppointmentStatus notification to Customer no:{customerNumber}");
            }
            else
            {
                logger.LogInformation($"IsCustomerWhitelistedByServiceprovider is false for Service Provider Id:{appointment.ServiceProviderId}");
            }
        }

        public async Task FireReminderNotification(string appointmentId)
        {
            var appointmentData = await GetAppointmentData(appointmentId);
            var appointment = appointmentData.Item1;
            var customerNumber = appointmentData.Item2;
            var spNumber = appointmentData.Item3;

            if (!appointment.ScheduledAppointmentStartTime.HasValue)
            {
                logger.LogError($"At FireReminderNotification ScheduledAppointmentStartTime for appointment id:{appointment.AppointmentId} is null");
                return;
            }

            //For customer
            if (WhiteList.IsCustomerWhitelistedByServiceprovider(appointment.ServiceProviderId))
            {
                var customerStatus = smsService.SendAppointmentReminderSMS(customerNumber, appointment.ScheduledAppointmentStartTime.Value, appointment.CustomerName);
                logger.LogInformation($"Reminder Notification status for {customerNumber} = {customerStatus}");
            }

            //For service provider
            if (WhiteList.IsServiceproviderWhitelisted(appointment.ServiceProviderId))
            {
                var spStatus = smsService.SendAppointmentReminderSMS(
                    spNumber,
                    appointment.ScheduledAppointmentStartTime.Value,
                    appointment.ServiceProviderName);

                logger.LogInformation($"Reminder Notification status for {spNumber} = {spStatus}");
            }
        }



        //Private methods, TODO move somewhere



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

        private void SendAppointmentStatusSmsToServiceProvider(string doctorPhoneNumber, DateTime appointmentTime, string custName, string status)
        {
            var sentStatus = smsService.SendAppointmentStatusSMS(doctorPhoneNumber, appointmentTime, custName, status);
            logger.LogInformation($"Appointment status Notification status for {doctorPhoneNumber} = {sentStatus}");
        }

        private void SendAppointmentStatusSmsToCustomer(string customerPhoneNumber, DateTime appointmentTime, string spName, string status)
        {
            var sentStatus = smsService.SendAppointmentStatusSMS(customerPhoneNumber, appointmentTime, spName, status);
            logger.LogInformation($"Appointment status Notification status for {customerPhoneNumber} = {sentStatus}");
        }
    }
}
