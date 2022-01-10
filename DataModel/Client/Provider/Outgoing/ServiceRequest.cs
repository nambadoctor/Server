namespace DataModel.Client.Provider.Outgoing
{
    public class ServiceRequest
    {
        public string ServiceRequestId { get; set; }
        public string CustomerId { get; set; }
        public string OrganisationId { get; set; }
        public string ServiceProviderId { get; set; }
        public string AppointmentId { get; set; }
        public string Reason { get; set; }
    }
}
