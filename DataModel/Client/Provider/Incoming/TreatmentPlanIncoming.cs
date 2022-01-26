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
        public List<TreatmentIncoming> Treatments { get; set; }
    }
}
