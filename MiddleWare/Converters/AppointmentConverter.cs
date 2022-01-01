using Mongo = DataModel.Mongo;
using Client = DataModel.Client.Provider;

namespace MiddleWare.Converters
{
    public static class AppointmentConverter
    {
        public static Client.Appointment ConvertToClientAppointmentData(
            Mongo.ServiceProviderProfile serviceProviderProfile,
            Mongo.Appointment appointment,
            Mongo.CustomerProfile customerProfile)
        {
            var appointmentData = new Client.Appointment();

            appointmentData.ServiceProviderId = serviceProviderProfile.ServiceProviderId;
            appointmentData.ServiceProviderName = $"Dr. {serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";

            appointmentData.CustomerId = customerProfile.CustomerId;
            appointmentData.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

            appointmentData.AppointmentId = appointment.AppointmentId.ToString();
            appointmentData.AppointmentType = appointment.AppointmentType.ToString();
            appointmentData.Status = appointment.Status.ToString();
            appointmentData.ScheduledAppointmentStartTime = appointment.ScheduledAppointmentStartTime;
            appointmentData.ScheduledAppointmentEndTime = appointment.ScheduledAppointmentEndTime;
            appointmentData.ActualAppointmentStartTime = appointment.ActualAppointmentStartTime;
            appointmentData.ActualAppointmentEndTime = appointment.ActualAppointmentEndTime;

            return appointmentData;
        }
    }
}
