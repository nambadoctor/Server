using DataModel.Mongo;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface ITreatmentPlanRepository : IRepository<TreatmentPlan>
    {
        public Task<TreatmentPlan> GetTreatmentPlanByServiceRequestId(string ServiceRequestId);
        public Task<List<TreatmentPlan>> GetAllTreatmentPlans(string OrganisationId, string? ServiceProviderId = null, string? CustomerId = null);
        public Task UpsertTreatmentPlan(TreatmentPlan treatmentPlan);
        public Task AddTreatment(string TreatmentPlanId, Treatment treatment);
        public Task UpdateTreatment(string TreatmentPlanId, Treatment treatment);
        public Task RemoveTreatment(string TreatmentPlanId, string TreatmentId);
        public Task AddTreatmentPlanDocument(FileInfo fileInfo, string TreatmentPlanId);
        public Task DeleteTreatmentPlanDocument(string TreatmentPlanDocumentId);
    }
}
