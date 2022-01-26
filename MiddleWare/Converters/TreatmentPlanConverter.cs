using Mongo = DataModel.Mongo;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public static class TreatmentPlanConverter
    {

        public static List<ProviderClientOutgoing.TreatmentPlanOutgoing> ConvertToOutgoingTreatmentPlanList(List<Mongo.TreatmentPlan> mongoTreatmentPlans)
        {
            var treatmentPlans = new List<ProviderClientOutgoing.TreatmentPlanOutgoing>();

            if (mongoTreatmentPlans != null)
            {
                foreach (var treatmentPlan in mongoTreatmentPlans)
                {
                    treatmentPlans.Add(ConvertToOutgoingTreatmentPlan(treatmentPlan));
                }
            }

            return treatmentPlans;
        }
        public static ProviderClientOutgoing.TreatmentPlanOutgoing ConvertToOutgoingTreatmentPlan(Mongo.TreatmentPlan mongoTreatmentPlan)
        {
            var treatmentPlan = new ProviderClientOutgoing.TreatmentPlanOutgoing();

            treatmentPlan.CreatedDateTime = mongoTreatmentPlan.CreatedDateTime;

            treatmentPlan.OrganisationId = mongoTreatmentPlan.OrganisationId;

            treatmentPlan.ServiceProviderId = mongoTreatmentPlan.ServiceProviderId;

            treatmentPlan.CustomerId = mongoTreatmentPlan.CustomerId;

            treatmentPlan.ServiceProviderName = mongoTreatmentPlan.ServiceProviderName;

            treatmentPlan.CustomerName = mongoTreatmentPlan.CustomerName;

            treatmentPlan.OriginServiceRequestId = mongoTreatmentPlan.OriginServiceRequestId;

            //TODO Treatments list

            return treatmentPlan;
        }

        public static List<ProviderClientOutgoing.TreatmentOutgoing> ConvertToOutgoingTreatmentList(List<Mongo.Treatment> mongoTreatments)
        {
            var treatments = new List<ProviderClientOutgoing.TreatmentOutgoing>();

            if (mongoTreatments != null)
            {
                foreach (var treatment in mongoTreatments)
                {
                    treatments.Add(ConvertToOutgoingTreatment(treatment));
                }
            }

            return treatments;
        }

        public static ProviderClientOutgoing.TreatmentOutgoing ConvertToOutgoingTreatment(Mongo.Treatment mongoTreatment)
        {
            var treatment = new ProviderClientOutgoing.TreatmentOutgoing();

            treatment.TreatmentId = mongoTreatment.TreatmentId.ToString();

            if (mongoTreatment.TreatmentDetail != null)
            {
                treatment.Name = mongoTreatment.TreatmentDetail.Name;
                treatment.Description = mongoTreatment.TreatmentDetail.Description;
            }

            treatment.CreatedDateTime = mongoTreatment.CreatedDateTime;

            treatment.PlannedDateTime = mongoTreatment.PlannedDateTime;

            treatment.Status = mongoTreatment.Status.ToString();

            treatment.AppointmentId = mongoTreatment.AppointmentId;

            treatment.ServiceRequestId = mongoTreatment.ServiceRequestId;

            return treatment;
        }

        public static Mongo.Treatment ConvertToMongoTreatment(ProviderClientIncoming.TreatmentIncoming treatmentIncoming)
        {
            var treatment = new Mongo.Treatment();

            if (string.IsNullOrWhiteSpace(treatmentIncoming.TreatmentId))
            {
                treatment.TreatmentId = new ObjectId(treatmentIncoming.TreatmentId);
            }
            else
            {
                treatment.TreatmentId = ObjectId.GenerateNewId();
            }

            treatment.ServiceRequestId = treatmentIncoming.ServiceRequestId;

            treatment.AppointmentId = treatmentIncoming.AppointmentId;

            treatment.CreatedDateTime = DateTime.UtcNow;

            treatment.PlannedDateTime = treatmentIncoming.PlannedDateTime;

            Enum.TryParse(treatmentIncoming.Status, out Mongo.TreatmentStatus treatmentStatus);
            treatment.Status = treatmentStatus;

            treatment.TreatmentDetail = new Mongo.TreatmentDetail();
            treatment.TreatmentDetail.Description = treatmentIncoming.Description;
            treatment.TreatmentDetail.Name = treatmentIncoming.Name;

            return treatment;
        }
    }
}
