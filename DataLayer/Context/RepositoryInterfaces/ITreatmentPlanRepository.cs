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
        public Task<List<TreatmentPlan>> GetAllTreatmentPlans(string OrganisationId, string ServiceProviderId);
        public Task<List<TreatmentPlan>> GetTreatmentPlansOfCustomer(string OrganisationId, string CustomerId);
        public Task UpsertTreatmentPlan(TreatmentPlan treatmentPlan);
        public Task AddTreatment(string TreatmentPlanId, Treatment treatment);
        public Task UpdateTreatment(string TreatmentPlanId, Treatment treatment);
        public Task RemoveTreatment(string TreatmentPlanId, string TreatmentId);
    }
}
