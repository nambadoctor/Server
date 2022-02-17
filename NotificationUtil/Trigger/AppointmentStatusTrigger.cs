using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using Notification.Sms;
using NotificationUtil.Mode.SMS;

namespace NotificationUtil.Trigger
{
    public class AppointmentStatusTrigger : IAppointmentStatusTrigger
    {
        private ISmsService smsService;
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmentRepository;
        private ILogger logger;
        public AppointmentStatusTrigger(ISmsService smsService, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository, ILogger<AppointmentStatusTrigger> logger)
        {
            this.smsService = smsService;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmentRepository = appointmentRepository;
            this.logger = logger;
        }

        public async void FireAppointmentStatusNotification(string appointmentId)
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

            logger.LogInformation($"Firing notification to Customer no:{custPhoneNumber.Number} Service Provider Number:{spPhoneNumber.Number}");

            SendAppointmentStatusSmsToServiceProvider(
                spPhoneNumber.CountryCode.Replace("+", "") + spPhoneNumber.Number,
                appointment.ScheduledAppointmentStartTime.Value,
                appointment.CustomerName,
                appointment.Status.ToString()
                );

            SendAppointmentStatusSmsToCustomer(
                custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number,
                appointment.ScheduledAppointmentStartTime.Value,
                "Dr. " + appointment.ServiceProviderName,
                appointment.Status.ToString()
                );
        }

        private void SendAppointmentStatusSmsToServiceProvider(string doctorPhoneNumber, DateTime appointmentTime, string custName, string status)
        {
            if (SMSSpWhiteList.SpWhiteList.Contains(doctorPhoneNumber))
            {
                var sentStatus = smsService.SendAppointmentStatusSMS(doctorPhoneNumber, appointmentTime.ToString("MMM dd, HH:mm"), custName, status);
                logger.LogInformation($"Notification status for {doctorPhoneNumber} = {sentStatus}");
            }
            else
            {
                logger.LogInformation($"Doctor number {doctorPhoneNumber} not whitelisted");
            }
        }

        private void SendAppointmentStatusSmsToCustomer(string customerPhoneNumber, DateTime appointmentTime, string spName, string status)
        {
            var sentStatus = smsService.SendAppointmentStatusSMS(customerPhoneNumber, appointmentTime.ToString("MMM dd, HH:mm"), spName, status);
            logger.LogInformation($"Notification status for {customerPhoneNumber} = {sentStatus}");
        }
    }
}
