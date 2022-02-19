using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class TreatmentPlanRepository : BaseRepository<TreatmentPlan>, ITreatmentPlanRepository
    {
        public TreatmentPlanRepository(IMongoContext context) : base(context)
        {
        }

        public async Task UpsertTreatmentPlan(TreatmentPlan treatmentPlan)
        {
            var filter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.TreatmentPlanId, treatmentPlan.TreatmentPlanId);

            var update = Builders<TreatmentPlan>.Update.Set(tp => tp.TreatmentPlanId, treatmentPlan.TreatmentPlanId);

            update = update.Set(tp => tp.Treatments, treatmentPlan.Treatments);

            update = update.Set(tp => tp.TreatmentPlanStatus, treatmentPlan.TreatmentPlanStatus);

            update = update.Set(tp => tp.TreatmentPlanName, treatmentPlan.TreatmentPlanName);

            await this.Upsert(filter, update);
        }

        public async Task<List<TreatmentPlan>> GetAllTreatmentPlans(string OrganisationId, string? ServiceProviderId, string? CustomerId = null)
        {
            var organisationFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.OrganisationId, OrganisationId);

            FilterDefinition<TreatmentPlan> combinedFilter = organisationFilter; //Default filter
            
            if (!string.IsNullOrWhiteSpace(ServiceProviderId))
            {
                var serviceProviderFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.ServiceProviderId, ServiceProviderId);
                combinedFilter &= serviceProviderFilter;
            }
            
            if (!string.IsNullOrWhiteSpace(CustomerId))
            {
                var customerFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.CustomerId, CustomerId);
                combinedFilter &= customerFilter;
            }

            var result = await this.GetListByFilter(combinedFilter);

            return result.ToList();
        }

        public async Task AddTreatment(string TreatmentPlanId, Treatment treatment)
        {
            var filter = Builders<TreatmentPlan>.Filter.Eq(sp => sp.TreatmentPlanId, new ObjectId(TreatmentPlanId));

            var update = Builders<TreatmentPlan>.Update.AddToSet(sp => sp.Treatments, treatment);

            await this.AddToSet(filter, update);
        }

        public async Task RemoveTreatment(string TreatmentPlanId, string TreatmentId)
        {
            var filter = Builders<TreatmentPlan>.Filter.Eq(sp => sp.TreatmentPlanId, new ObjectId(TreatmentPlanId));

            var update = Builders<TreatmentPlan>.Update.PullFilter(sp => sp.Treatments, treatment => treatment.TreatmentId == new ObjectId(TreatmentId));

            await this.RemoveFromSet(filter, update);
        }

        public async Task UpdateTreatment(string TreatmentPlanId, Treatment treatment)
        {
            var filter = Builders<TreatmentPlan>.Filter;

            var nestedFilter = filter.ElemMatch(
                tp => tp.Treatments,
                tr => tr.TreatmentId.Equals(treatment.TreatmentId));

            var update = Builders<TreatmentPlan>.Update.Set(tp => tp.TreatmentPlanId, new ObjectId(TreatmentPlanId));

            if (!string.IsNullOrEmpty(treatment.Name))
            {
                update = update.Set("Treatments.$.Name", treatment.Name);
            }

            if (!string.IsNullOrEmpty(treatment.ActualProcedure))
            {
                update = update.Set("Treatments.$.ActualProcedure", treatment.ActualProcedure);
            }

            if (!string.IsNullOrEmpty(treatment.TreatmentInstanceServiceRequestId))
            {
                update = update.Set("Treatments.$.TreatmentInstanceServiceRequestId", treatment.TreatmentInstanceServiceRequestId);
            }

            if (!string.IsNullOrEmpty(treatment.TreatmentInstanceAppointmentId))
            {
                update = update.Set("Treatments.$.TreatmentInstanceAppointmentId", treatment.TreatmentInstanceAppointmentId);
            }

            update = update.Set("Treatments.$.ActualDateTime", treatment.ActualDateTime);

            update = update.Set("Treatments.$.Status", treatment.Status);











            await this.Upsert(nestedFilter, update);
        }

    }
}
