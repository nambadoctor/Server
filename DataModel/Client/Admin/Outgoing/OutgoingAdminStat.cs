using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Admin.Outgoing
{
    public class OutgoingAdminStat
    {
        public string ServiceProviderName { get; set; }
        public string OrganisationName { get; set; }
        public int NoOfAppointments { get; set; }
        public int NoOfDocumentsUploaded { get; set; }
    }
}
