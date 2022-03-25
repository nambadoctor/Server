using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class ReferralIncoming
    {
        public string CustomerId { get; set; }
        public string SenderServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
    }
}
