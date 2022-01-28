using DataModel.Client.Provider.Incoming;
using DataModel.Client.Provider.Outgoing;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using Exceptions = DataModel.Shared.Exceptions;
using MongoDB.GenericRepository.Interfaces;
using Mongo = DataModel.Mongo;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;

namespace MiddleWare.Services
{
    public class TreatmentPlanService : ITreatmentPlanService
    {
        private ILogger logger;
        private ITreatmentPlanRepository treatmentPlanRepository;

        public TreatmentPlanService(ITreatmentPlanRepository treatmentPlanRepository, ILogger<TreatmentPlanService> logger)
        {
            this.treatmentPlanRepository = treatmentPlanRepository;
            this.logger = logger;
        }

        public Task AddTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            throw new NotImplementedException();
        }

        public Task AddTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTreatment(string TreatmentPlanId, string TreatmentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TreatmentPlanOutgoing>> GetAllTreatmentPlans(string OrganisationId, string ServiceproviderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TreatmentPlanOutgoing>> GetCustomerTreatmentPlans(string OrganisationId, string CustomerId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            throw new NotImplementedException();
        }
    }
}
