﻿using Mongo = DataModel.Mongo;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public static class AppointmentConverter
    {

        public static List<ProviderClientOutgoing.OutgoingAppointment> ConvertToClientAppointmentDataList(
            List<(Mongo.Appointment, string, int, int, int)> appointments)
        {
            var listToReturn = new List<ProviderClientOutgoing.OutgoingAppointment>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    listToReturn.Add(ConvertToClientAppointmentData(
                        appointment.Item1, appointment.Item2,
                        appointment.Item3, 
                        appointment.Item4,
                        appointment.Item5)
                    );
                }
            }
            return listToReturn;
        }
        public static ProviderClientOutgoing.OutgoingAppointment ConvertToClientAppointmentData(
            Mongo.Appointment appointment, string phoneNumber, int numberOfNotes, int numberOfReports, int numberOfPrescriptionDocuments)
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

            appointmentData.PhoneNumber = phoneNumber;
            appointmentData.NumberOfNotes = numberOfNotes;
            appointmentData.NumberOfReports = numberOfReports;
            appointmentData.NumberOfPrescriptionDocuments = numberOfPrescriptionDocuments;

            return appointmentData;
        }

        public static Mongo.Appointment ConvertToMongoAppointmentData(
            Mongo.ServiceProviderProfile serviceProviderProfile,
           ProviderClientIncoming.AppointmentIncoming appointment,
           Mongo.CustomerProfile customerProfile)
        {
            var appointmentData = new Mongo.Appointment();

            if (string.IsNullOrWhiteSpace(appointment.AppointmentId))
            {
                appointmentData.AppointmentId = ObjectId.GenerateNewId();
            }
            else
            {
                appointmentData.AppointmentId = new ObjectId(appointment.AppointmentId);
            }

            appointmentData.ServiceProviderId = appointment.ServiceProviderId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

            appointmentData.OrganisationId = appointment.OrganisationId;

            appointmentData.CustomerId = appointment.CustomerId;

            appointmentData.ServiceRequestId = appointment.ServiceRequestId;

            appointmentData.ServiceProviderName = $"{serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}";

            appointmentData.CustomerName = $"{customerProfile.FirstName} {customerProfile.LastName}";

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
