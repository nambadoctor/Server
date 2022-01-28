using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class TreatmentPlanOutgoing
    {
        public string TreatmentPlanId { get; set; }
        public string TreatmentPlanName { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceProviderName { get; set; }
        public string OriginServiceRequestId { get; set; }
        public List<TreatmentOutgoing> Treatments { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
