using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class TreatmentIncoming
    {
        public string TreatmentId { get; set; }
        public string AppointmentId { get; set; }
        public string ServiceRequestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PlannedDateTime { get; set; }
        public string Status { get; set; }
    }
}
