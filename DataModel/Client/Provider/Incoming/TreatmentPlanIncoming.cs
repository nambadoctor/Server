using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class TreatmentPlanIncoming
    {
        public string TreatmentPlanId { get; set; }
        public string TreatmentPlanName { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string CustomerId { get; set; }
        public string SourceServiceRequestId { get; set; }
        public string TreatmentPlanStatus { get; set; }
        public List<TreatmentIncoming> Treatments { get; set; }
    }
}
