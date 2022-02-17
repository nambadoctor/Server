using DataModel.Mongo;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.GenericRepository.Interfaces;
using Notification.Mode;
using Notification.Mode.SMS;

namespace Notification.Function
{
    public class ReminderFunction
    {
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmentRepository;
        private ISmsService smsService;

        private int REMINDER_INTERVAL_DAYS = 1;
        public ReminderFunction(IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmentRepository, ISmsService smsService)
        {
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmentRepository = appointmentRepository;
            this.smsService = smsService;
        }

        [FunctionName("AppointmentReminder")]//Run every day at 9:30 am
        public async Task Run([TimerTrigger("0 30 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            var appointmentRelevantData = await GetAllAppointmentsPendingData();

            if (appointmentRelevantData == null)
            {
                log.LogInformation("No pending appointments to remind today");
                return;
            }

            FireNotificationForCustomers(appointmentRelevantData);

            FireNotificationForServiceProviders(appointmentRelevantData);
        }

        private async Task<List<AppointmentRelevantData>?> GetAllAppointmentsPendingData()
        {
            var pendingAppointments = await appointmentRepository.GetAllAppointments(AppointmentStatus.Confirmed, DateTime.UtcNow, DateTime.UtcNow.AddDays(REMINDER_INTERVAL_DAYS));

            if (pendingAppointments == null || pendingAppointments.Count == 0)
                return null;

            var appointmentRelevantData = new List<AppointmentRelevantData>();

            var customerIds = pendingAppointments.Select(app => app.CustomerId).ToList();
            var serviceProviderIds = pendingAppointments.Select(app => app.ServiceProviderId).ToList();

            var relevantCustomers = await customerRepository.GetCustomerProfiles(customerIds);

            var relevantServiceProviders = await serviceProviderRepository.GetServiceProviderProfiles(serviceProviderIds);

            foreach (var appointment in pendingAppointments)
            {
                var spProfile = relevantServiceProviders.Find(sp => sp.ServiceProviderId == appointment.ServiceProviderId && sp.OrganisationId == appointment.OrganisationId);
                var custProfile = relevantCustomers.Find(cust => cust.CustomerId == appointment.CustomerId && cust.OrganisationId == appointment.OrganisationId);
                if (spProfile == null || custProfile == null)
                {
                    continue;
                    //TODO Do alert
                }
                var custPhoneNumber = custProfile.PhoneNumbers.First();
                var spPhoneNumber = spProfile.PhoneNumbers.First();

                appointmentRelevantData.Add(
                    new AppointmentRelevantData
                    {
                        CustomerName = appointment.CustomerName,
                        CustomerNumber = custPhoneNumber.CountryCode.Replace("+", "") + custPhoneNumber.Number,
                        ServiceProviderName = "Dr. " + appointment.ServiceProviderName,
                        ServiceProviderNumber = spPhoneNumber.CountryCode.Replace("+", "") + spPhoneNumber.Number,
                        ScheduledTime = appointment.ScheduledAppointmentStartTime.HasValue ? appointment.ScheduledAppointmentStartTime.Value.ToString("MMM dd, HH:mm") : DateTime.UtcNow.ToString("MMM dd, HH:mm")
                    });
            }

            return appointmentRelevantData;
        }

        private void FireNotificationForCustomers(List<AppointmentRelevantData> appointmentRelevantDatas)
        {
            foreach (var data in appointmentRelevantDatas)
            {
                smsService.SendAppointmentReminderSMS(data.CustomerName, data.ScheduledTime, data.ServiceProviderName);
            }
        }

        private void FireNotificationForServiceProviders(List<AppointmentRelevantData> appointmentRelevantDatas)
        {
            var dataForWhiteListedNumbers = appointmentRelevantDatas.FindAll(app => SMSSpWhiteList.SpWhiteList.Contains(app.ServiceProviderNumber));

            foreach (var data in dataForWhiteListedNumbers)
            {
                smsService.SendAppointmentReminderSMS(data.ServiceProviderNumber, data.ScheduledTime, data.CustomerName);
            }
        }
    }
}
