using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class ReportOutgoing
    {
        public string ReportId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; } //Pdf, Img
        public string SasUrl { get; set; }
        public string ServiceRequestId { get; set; }
        public string AppointmentId { get; set; }
        public DateTime? UploadedDateTime { get; set; }
    }
}
