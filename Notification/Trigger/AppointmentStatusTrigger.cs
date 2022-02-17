using MongoDB.GenericRepository.Interfaces;
using Notification.Mode;
using Notification.Mode.SMS;

namespace Notification.Trigger
{
    public class AppointmentStatusTrigger : IAppointmentStatusTrigger
    {
        private ISmsService smsService;
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmentRepository;
        public AppointmentStatusTrigger(ISmsService smsService, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository)
        {
            this.smsService = smsService;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmentRepository = appointmentRepository;
        }

        public async void FireAppointmentStatusNotification(string appointmentId)
        {
            var appointment = await appointmentRepository.GetAppointment(appointmentId);

            var customerProfile = await customerRepository.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

            var spProfile = await serviceProviderRepository.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

            var custPhoneNumber = customerProfile.PhoneNumbers.First();
            var spPhoneNumber = spProfile.PhoneNumbers.First();

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
                smsService.SendAppointmentStatusSMS(doctorPhoneNumber, appointmentTime.ToString("MMM dd, HH:mm"), custName, status);
        }

        private void SendAppointmentStatusSmsToCustomer(string customerPhoneNumber, DateTime appointmentTime, string spName, string status)
        {
            smsService.SendAppointmentStatusSMS(customerPhoneNumber, appointmentTime.ToString("MMM dd, HH:mm"), spName, status);
        }
    }
}
