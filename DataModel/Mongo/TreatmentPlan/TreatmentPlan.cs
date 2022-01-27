using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class TreatmentPlan
    {
        [BsonId]
        public ObjectId TreatmentPlanId { get; set; }
        public ObjectId TreatmentPlanName { get; set; }

        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceProviderName { get; set; }
        public string SourceServiceRequestId { get; set; }
        public List<Treatment> Treatments { get; set; }

        public string TreatmentPlanStatus { get; set; } // This is to represent aggreation of treatment status. This takes precedence over agregated results
        public DateTime CreatedDateTime { get; set; }
    }
}
