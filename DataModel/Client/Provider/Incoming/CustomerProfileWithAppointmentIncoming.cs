using DataModel.Client.Provider.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class CustomerProfileWithAppointmentIncoming
    {
        public string AppointmentType { get; set; }
        public string AddressId { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduledAppointmentStartTime { get; set; }
        public DateTime? ScheduledAppointmentEndTime { get; set; }
        public DateTime? ActualAppointmentStartTime { get; set; }
        public DateTime? ActualAppointmentEndTime { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string Gender { get; set; }
        public DateOfBirth DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ProfilePicURL { get; set; }
        public string? OrganisationId { get; set; }
        public string? ServiceProviderId { get; set; }
    }
}
