using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class NoteOutgoing
    {
        public string NoteId { get; set; }
        public string Note { get; set; }
        public string ServiceRequestId { get; set; }
        public string AppointmentId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
    }
}
