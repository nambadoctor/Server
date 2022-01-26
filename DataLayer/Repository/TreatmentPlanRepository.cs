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

            await this.Upsert(filter, update);
        }

        public async Task<List<TreatmentPlan>> GetAllTreatmentPlans(string OrganisationId, string ServiceProviderId)
        {
            var organisationFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.OrganisationId, OrganisationId);

            var serviceProviderFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.ServiceProviderId, ServiceProviderId);

            var combinedFilter = string.IsNullOrWhiteSpace(ServiceProviderId) ? organisationFilter : organisationFilter & serviceProviderFilter;

            var result = await this.GetListByFilter(combinedFilter);

            return result.ToList();
        }

        public async Task<List<TreatmentPlan>> GetTreatmentPlansOfCustomer(string OrganisationId, string CustomerId)
        {
            var organisationFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.OrganisationId, OrganisationId);

            var customerFilter = Builders<TreatmentPlan>.Filter.Eq(tp => tp.CustomerId, CustomerId);

            var combinedFilter = organisationFilter & customerFilter;

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

            var nestedFilter = Builders<TreatmentPlan>.Filter.ElemMatch(tp => tp.Treatments, treatment => treatment.TreatmentId.Equals(treatment.TreatmentId));

            var update = Builders<TreatmentPlan>.Update.Set(tp => tp.TreatmentPlanId, new ObjectId(TreatmentPlanId));

            update = update.Set("Treatments.$.TreatmentDetail", treatment.TreatmentDetail);

            update = update.Set("Treatments.$.ServiceRequestId", treatment.ServiceRequestId);

            update = update.Set("Treatments.$.AppointmentId", treatment.AppointmentId);

            update = update.Set("Treatments.$.PlannedDateTime", treatment.PlannedDateTime);

            update = update.Set("Treatments.$.Status", treatment.Status);

            await this.Upsert(nestedFilter, update);
        }

    }
}
