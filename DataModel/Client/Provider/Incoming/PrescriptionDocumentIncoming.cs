using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class PrescriptionDocumentIncoming
    {
        public string AppointmentId { get; set; }
        public string ServiceRequestId { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; } //Pdf, Img
        public string Details { get; set; }
        public string DetailsType { get; set; }
    }
}
