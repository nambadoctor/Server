using System;

namespace DataModel.Client.Provider.Outgoing
{
    public class GeneratedSlot
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get; set; }
        public string PaymentType { get; set; }
        public string AppointmentType { get; set; }
        public string AddressId { get; set; }
        public string OrganisationId { get; set; }
        public double? ServiceFees { get; set; }

    }
}
