using DataModel.Mongo;
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

        public async void FireAppointmentStatusNotification(string appointmentId)
        {
            var appointmentData = await GetAppointmentData(appointmentId);

            var appointment = appointmentData.Item1;
            var customerNumber = appointmentData.Item2;
            var spNumber = appointmentData.Item3;

            logger.LogInformation($"Firing notification to Customer no:{customerNumber} Service Provider Number:{spNumber}");

            SendAppointmentStatusSmsToServiceProvider(
                customerNumber,
                appointment.ScheduledAppointmentStartTime.Value.ToUniversalTime(),
                appointment.CustomerName,
                appointment.Status.ToString()
                );

            SendAppointmentStatusSmsToCustomer(
                spNumber,
                appointment.ScheduledAppointmentStartTime.Value.ToUniversalTime(),
                "Dr. " + appointment.ServiceProviderName,
                appointment.Status.ToString()
                );
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

        public async void FireReminderNotification(string appointmentId)
        {
            var appointmentData = await GetAppointmentData(appointmentId);


            //For customer
            var customerStatus = smsService.SendAppointmentReminderSMS(appointmentData.Item2, appointmentData.Item1.ScheduledAppointmentStartTime.Value, appointmentData.Item1.CustomerName);
            logger.LogInformation($"Reminder Notification status for {appointmentData.Item2} = {customerStatus}");

            //For service provider
            if (IsServiceProviderWhitelisted(appointmentData.Item3))
            {
                var spStatus = smsService.SendAppointmentReminderSMS(
                    appointmentData.Item3,
                    appointmentData.Item1.ScheduledAppointmentStartTime.Value,
                    appointmentData.Item1.ServiceProviderName);

                logger.LogInformation($"Reminder Notification status for {appointmentData.Item2} = {spStatus}");
            }
        }

        private void SendAppointmentStatusSmsToServiceProvider(string doctorPhoneNumber, DateTime appointmentTime, string custName, string status)
        {
            if (IsServiceProviderWhitelisted(doctorPhoneNumber))
            {
                var sentStatus = smsService.SendAppointmentStatusSMS(doctorPhoneNumber, appointmentTime, custName, status);
                logger.LogInformation($"Appointment status Notification status for {doctorPhoneNumber} = {sentStatus}");
            }
        }

        private void SendAppointmentStatusSmsToCustomer(string customerPhoneNumber, DateTime appointmentTime, string spName, string status)
        {
            var sentStatus = smsService.SendAppointmentStatusSMS(customerPhoneNumber, appointmentTime, spName, status);
            logger.LogInformation($"Appointment status Notification status for {customerPhoneNumber} = {sentStatus}");
        }

        private bool IsServiceProviderWhitelisted(string doctorPhoneNumber)
        {
            if (SMSSpWhiteList.SpWhiteList.Contains(doctorPhoneNumber))
            {
                logger.LogInformation($"Doctor number {doctorPhoneNumber} is whitelisted");
                return true;
            }
            else
            {
                logger.LogInformation($"Doctor number {doctorPhoneNumber} not whitelisted");
                return false;
            }
        }
    }
}
