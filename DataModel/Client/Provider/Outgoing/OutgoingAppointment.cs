using System;

namespace DataModel.Client.Provider.Outgoing
{
    public class OutgoingAppointment
    {
        public string AppointmentId { get; set; }
        public string OrganisationId { get; set; }
        public string ServiceRequestId { get; set; }
        public string ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AppointmentType { get; set; }
        public string AddressId { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduledAppointmentStartTime { get; set; }
        public DateTime? ScheduledAppointmentEndTime { get; set; }
        public DateTime? ActualAppointmentStartTime { get; set; }
        public DateTime? ActualAppointmentEndTime { get; set; }
    }
}
