using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class TreatmentOutgoing
    {
        public string TreatmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PlannedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string AppointmentId { get; set; }
        public string ServiceRequestId { get; set; }
        public string Status { get; set; }
    }
}
