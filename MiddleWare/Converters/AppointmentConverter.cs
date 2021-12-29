using Mongo = DataModel.Mongo;
using Client = DataModel.Client.Provider;

namespace MiddleWare.Converters
{
    public static class AppointmentConverter
    {
        public static Client.AppointmentData ConvertToClientAppointmentData(
            string serviceProviderId,
            Mongo.ServiceProviderProfile serviceProviderProfile,
            Mongo.Appointment appointment,
            string customerId,
            Mongo.CustomerProfile customerProfile,
            Mongo.ServiceRequest serviceRequest)
        {
            var appointmentData = new Client.AppointmentData();

            appointmentData.ServiceProviderId = serviceProviderId;
            appointmentData.ServiceProviderName = $"Dr. {serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";

            appointmentData.CustomerId = customerId;
            appointmentData.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

            appointmentData.AppointmentId = appointment.AppointmentId.ToString();
            appointmentData.AppointmentType = appointment.AppointmentType;
            appointmentData.Status = appointment.Status;
            appointmentData.ScheduledAppointmentStartTime = appointment.ScheduledAppointmentStartTime;
            appointmentData.ScheduledAppointmentEndTime = appointment.ScheduledAppointmentEndTime;
            appointmentData.ActualAppointmentStartTime = appointment.ActualAppointmentStartTime;
            appointmentData.ActualAppointmentEndTime = appointment.ActualAppointmentEndTime;

            appointmentData.ServiceRequest = serviceRequest;

            return appointmentData;
        }
    }
}
