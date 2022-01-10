using Mongo = DataModel.Mongo;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public static class AppointmentConverter
    {
        public static ProviderClientOutgoing.OutgoingAppointment ConvertToClientAppointmentData(
            Mongo.ServiceProviderProfile serviceProviderProfile,
            Mongo.Appointment appointment,
            Mongo.CustomerProfile customerProfile)
        {
            var appointmentData = new ProviderClientOutgoing.OutgoingAppointment();

            appointmentData.ServiceProviderId = serviceProviderProfile.ServiceProviderId;
            appointmentData.ServiceProviderName = $"Dr. {serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";

            appointmentData.CustomerId = customerProfile.CustomerId;
            appointmentData.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

            appointmentData.AppointmentId = appointment.AppointmentId.ToString();
            appointmentData.OrganisationId = appointment.OrganisationId;
            appointmentData.ServiceRequestId = appointment.ServiceRequestId;
            appointmentData.AddressId = appointment.AddressId;
            appointmentData.AppointmentType = appointment.AppointmentType.ToString();
            appointmentData.Status = appointment.Status.ToString();
            appointmentData.ScheduledAppointmentStartTime = appointment.ScheduledAppointmentStartTime;
            appointmentData.ScheduledAppointmentEndTime = appointment.ScheduledAppointmentEndTime;
            appointmentData.ActualAppointmentStartTime = appointment.ActualAppointmentStartTime;
            appointmentData.ActualAppointmentEndTime = appointment.ActualAppointmentEndTime;

            return appointmentData;
        }

        public static ProviderClientOutgoing.OutgoingAppointment ConvertToClientAppointmentData(
            string serviceProviderName,
            Mongo.Appointment appointment,
            string customerName)
        {
            var appointmentData = new ProviderClientOutgoing.OutgoingAppointment();

            appointmentData.ServiceProviderId = appointment.ServiceProviderId;
            appointmentData.ServiceProviderName = serviceProviderName;

            appointmentData.CustomerId = appointment.CustomerId;
            appointmentData.CustomerName = customerName;

            appointmentData.AddressId = appointment.AddressId;
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
           ProviderClientIncoming.AppointmentIncoming appointment)
        {
            var appointmentData = new Mongo.Appointment();

            appointmentData.ServiceProviderId = appointment.ServiceProviderId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

            appointmentData.OrganisationId = appointment.OrganisationId;

            appointmentData.CustomerId = appointment.CustomerId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

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
