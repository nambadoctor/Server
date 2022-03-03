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

            treatmentPlan.OriginServiceRequestId = mongoTreatmentPlan.SourceServiceRequestId;

            treatmentPlan.TreatmentPlanName = mongoTreatmentPlan.TreatmentPlanName;

            treatmentPlan.Treatments = ConvertToOutgoingTreatmentList(mongoTreatmentPlan.Treatments, mongoTreatmentPlan);

            treatmentPlan.TreatmentPlanId = mongoTreatmentPlan.TreatmentPlanId.ToString();

            return treatmentPlan;
        }

        public static List<ProviderClientOutgoing.TreatmentOutgoing> ConvertToDenormalizedTreatments(List<Mongo.TreatmentPlan> mongoTreatmentPlans)
        {
            var treatments = new List<ProviderClientOutgoing.TreatmentOutgoing>();

            if (mongoTreatmentPlans != null)
            {
                foreach (var treatmentPlan in mongoTreatmentPlans)
                {
                    treatments.AddRange(ConvertToOutgoingTreatmentPlan(treatmentPlan).Treatments);
                }
            }

            return treatments;
        }

        public static List<ProviderClientOutgoing.TreatmentOutgoing> ConvertToOutgoingTreatmentList(List<Mongo.Treatment> mongoTreatments, Mongo.TreatmentPlan mongoTreatmentPlan)
        {
            var treatments = new List<ProviderClientOutgoing.TreatmentOutgoing>();

            if (mongoTreatments != null)
            {
                foreach (var treatment in mongoTreatments)
                {
                    treatments.Add(ConvertToOutgoingTreatment(treatment, mongoTreatmentPlan));
                }
            }

            return treatments;
        }

        public static ProviderClientOutgoing.TreatmentOutgoing ConvertToOutgoingTreatment(Mongo.Treatment mongoTreatment, Mongo.TreatmentPlan mongoTreatmentPlan)
        {
            var treatment = new ProviderClientOutgoing.TreatmentOutgoing();

            treatment.TreatmentId = mongoTreatment.TreatmentId.ToString();

            treatment.Name = mongoTreatment.Name;

            treatment.OriginalInstructions = mongoTreatment.OrginalInstructions;

            treatment.CreatedDateTime = mongoTreatment.CreatedDateTime;

            treatment.PlannedDateTime = mongoTreatment.PlannedDateTime;

            treatment.Status = mongoTreatment.Status.ToString();

            treatment.AppointmentId = mongoTreatment.TreatmentInstanceAppointmentId;

            treatment.ServiceRequestId = mongoTreatment.TreatmentInstanceServiceRequestId;

            treatment.ActualProcedure = mongoTreatment.ActualProcedure;

            treatment.TreatmentPlanId = mongoTreatmentPlan.TreatmentPlanId.ToString();

            treatment.TreatmentPlanName = mongoTreatmentPlan.TreatmentPlanName;

            treatment.CustomerName = mongoTreatmentPlan.CustomerName;

            treatment.ServiceProviderName = mongoTreatmentPlan.ServiceProviderName;

            treatment.ServiceProviderId = mongoTreatmentPlan.ServiceProviderId;

            treatment.CustomerId = mongoTreatmentPlan.CustomerId;

            return treatment;
        }

        public static Mongo.Treatment ConvertToMongoTreatment(ProviderClientIncoming.TreatmentIncoming treatmentIncoming)
        {
            var treatment = new Mongo.Treatment();

            if (!string.IsNullOrWhiteSpace(treatmentIncoming.TreatmentId))
            {
                treatment.TreatmentId = new ObjectId(treatmentIncoming.TreatmentId);
            }
            else
            {
                treatment.TreatmentId = ObjectId.GenerateNewId();
            }

            treatment.TreatmentInstanceServiceRequestId = treatmentIncoming.ServiceRequestId;

            treatment.TreatmentInstanceAppointmentId = treatmentIncoming.AppointmentId;

            treatment.CreatedDateTime = DateTime.UtcNow;

            treatment.PlannedDateTime = treatmentIncoming.PlannedDateTime;

            Enum.TryParse(treatmentIncoming.Status, out Mongo.TreatmentStatus treatmentStatus);
            treatment.Status = treatmentStatus;

            treatment.OrginalInstructions = treatmentIncoming.OrginalInstructions;
            treatment.Name = treatmentIncoming.Name;

            treatment.ActualProcedure = treatmentIncoming.ActualProcedure;

            return treatment;
        }

        public static List<Mongo.Treatment> ConvertToMongoTreatmentList(List<ProviderClientIncoming.TreatmentIncoming> treatmentsIncomings)
        {
            var listToReturn = new List<Mongo.Treatment>();

            if (treatmentsIncomings != null)
                foreach (var treatment in treatmentsIncomings)
                {
                    listToReturn.Add(ConvertToMongoTreatment(treatment));
                }

            return listToReturn;
        }

        public static Mongo.TreatmentPlan ConvertToMongoTreatmentPlan(ProviderClientIncoming.TreatmentPlanIncoming treatmentPlan, string? ServiceProviderName, string? CustomerName)
        {
            var mongoTreatmentPlan = new Mongo.TreatmentPlan();

            if (string.IsNullOrWhiteSpace(treatmentPlan.TreatmentPlanId))
            {
                mongoTreatmentPlan.TreatmentPlanId = ObjectId.GenerateNewId();
            }
            else
            {
                mongoTreatmentPlan.TreatmentPlanId = new ObjectId(treatmentPlan.TreatmentPlanId);
            }

            mongoTreatmentPlan.Treatments = ConvertToMongoTreatmentList(treatmentPlan.Treatments);

            mongoTreatmentPlan.CreatedDateTime = DateTime.UtcNow;

            mongoTreatmentPlan.CustomerId = treatmentPlan.CustomerId;

            mongoTreatmentPlan.OrganisationId = treatmentPlan.OrganisationId;

            mongoTreatmentPlan.ServiceProviderId = treatmentPlan.ServiceProviderId;

            mongoTreatmentPlan.ServiceProviderName = ServiceProviderName;

            mongoTreatmentPlan.CustomerName = CustomerName;

            mongoTreatmentPlan.SourceServiceRequestId = treatmentPlan.SourceServiceRequestId;

            mongoTreatmentPlan.TreatmentPlanName = treatmentPlan.TreatmentPlanName;

            mongoTreatmentPlan.TreatmentPlanStatus = treatmentPlan.TreatmentPlanStatus;

            return mongoTreatmentPlan;
        }
    }
}
