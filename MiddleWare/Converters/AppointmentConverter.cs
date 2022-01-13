using Mongo = DataModel.Mongo;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public static class AppointmentConverter
    {
        public static ProviderClientOutgoing.OutgoingAppointment ConvertToClientAppointmentData(
            Mongo.Appointment appointment)
        {
            var appointmentData = new ProviderClientOutgoing.OutgoingAppointment();

            appointmentData.ServiceProviderId = appointment.ServiceProviderId;
            appointmentData.ServiceProviderName = appointment.ServiceProviderName;

            appointmentData.CustomerId = appointment.CustomerId;
            appointmentData.CustomerName = appointment.CustomerName;

            appointmentData.AppointmentId = appointment.AppointmentId.ToString();
            appointmentData.OrganisationId = appointment.OrganisationId;
            appointmentData.ServiceRequestId = appointment.ServiceRequestId;
            appointmentData.AppointmentType = appointment.AppointmentType.ToString();
            appointmentData.Status = appointment.Status.ToString();
            appointmentData.ScheduledAppointmentStartTime = appointment.ScheduledAppointmentStartTime;
            appointmentData.ScheduledAppointmentEndTime = appointment.ScheduledAppointmentEndTime;
            appointmentData.ActualAppointmentStartTime = appointment.ActualAppointmentStartTime;
            appointmentData.ActualAppointmentEndTime = appointment.ActualAppointmentEndTime;

            return appointmentData;
        }

        public static Mongo.Appointment ConvertToMongoAppointmentData(
            Mongo.ServiceProviderProfile serviceProviderProfile,
           ProviderClientIncoming.AppointmentIncoming appointment,
           Mongo.CustomerProfile customerProfile)
        {
            var appointmentData = new Mongo.Appointment();

            appointmentData.ServiceProviderId = appointment.ServiceProviderId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

            appointmentData.OrganisationId = appointment.OrganisationId;

            appointmentData.CustomerId = appointment.CustomerId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

            appointmentData.ServiceProviderName = $"Dr. {serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";

            appointmentData.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

            if (!string.IsNullOrWhiteSpace(appointment.AppointmentId))
                appointmentData.AppointmentId = new ObjectId(appointment.AppointmentId);

            Enum.TryParse(appointment.AppointmentType, out Mongo.AppointmentType appointmentType);
            appointmentData.AppointmentType = appointmentType;

            Enum.TryParse(appointment.Status, out Mongo.AppointmentStatus statusType);
            appointmentData.Status = statusType;

            appointmentData.ScheduledAppointmentStartTime = appointment.ScheduledAppointmentStartTime;
            appointmentData.ScheduledAppointmentEndTime = appointment.ScheduledAppointmentEndTime;
            appointmentData.ActualAppointmentStartTime = appointment.ActualAppointmentStartTime;
            appointmentData.ActualAppointmentEndTime = appointment.ActualAppointmentEndTime;

            return appointmentData;
        }
    }
}
