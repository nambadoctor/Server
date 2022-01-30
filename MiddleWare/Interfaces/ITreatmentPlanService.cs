using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface ITreatmentPlanService
    {
        public Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetAllTreatmentPlans(string OrganisationId, string ServiceproviderId);
        public Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetCustomerTreatmentPlans(string OrganisationId, string CustomerId);
        public Task AddTreatmentPlan(ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming);
        public Task UpdateTreatmentPlan(ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming);
        public Task AddTreatment(string TreatmentPlanId, ProviderClientIncoming.TreatmentIncoming treatmentIncoming);
        public Task UpdateTreatment(string TreatmentPlanId, ProviderClientIncoming.TreatmentIncoming treatmentIncoming);
        public Task DeleteTreatment(string TreatmentPlanId, string TreatmentId);
    }
}
