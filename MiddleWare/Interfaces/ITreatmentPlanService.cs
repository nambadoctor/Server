using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface ITreatmentPlanService
    {
        public Task<List<ProviderClientOutgoing.TreatmentPlanOutgoing>> GetTreatmentPlans(string OrganisationId, string ServiceproviderId, string? CustomerId);
        public Task<List<ProviderClientOutgoing.TreatmentOutgoing>> GetTreatments(string OrganisationId, string ServiceproviderId, string CustomerId, bool IsUpcoming);
        public Task AddTreatmentPlan(ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming);
        public Task UpdateTreatmentPlan(ProviderClientIncoming.TreatmentPlanIncoming treatmentPlanIncoming);
        public Task AddTreatment(string TreatmentPlanId, ProviderClientIncoming.TreatmentIncoming treatmentIncoming);
        public Task UpdateTreatment(string TreatmentPlanId, ProviderClientIncoming.TreatmentIncoming treatmentIncoming);
        public Task DeleteTreatment(string TreatmentPlanId, string TreatmentId);
        public Task<List<ProviderClientOutgoing.TreatmentPlanDocumentsOutgoing>> GetTreatmentPlanDocuments(string ServiceRequestId);
        public Task<List<ProviderClientOutgoing.TreatmentPlanDocumentsOutgoing>> GetTreatmentPlanDocumentsOfCustomer(string SetCustomerId);
        public Task SetTreatmentPlanDocument(ProviderClientIncoming.TreatmentPlanDocumentIncoming treatmentPlanDocumentIncoming);
        public Task DeleteTreatmentPlanDocument(string TreatmentPlanDocumentId);
    }
}
