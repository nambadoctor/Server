using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class PrescriptionDocumentOutgoing
    {
        public string PrescriptionDocumentId { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; } //Pdf, Img
        public string Details { get; set; }
        public string DetailsType { get; set; }
        public string SasUrl { get; set; }
    }
}
