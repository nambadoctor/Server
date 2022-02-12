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
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;

        public TreatmentPlanService(ITreatmentPlanRepository treatmentPlanRepository, ILogger<TreatmentPlanService> logger, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository)
        {
            this.treatmentPlanRepository = treatmentPlanRepository;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.logger = logger;
        }

        public async Task AddTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:AddTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);

                var mongoTreatment = TreatmentPlanConverter.ConvertToMongoTreatment(treatmentIncoming);

                logger.LogInformation("Constructed mongo treatment plan obj");

                await treatmentPlanRepository.AddTreatment(TreatmentPlanId, mongoTreatment);

                logger.LogInformation($"Added treatment with id: {mongoTreatment.TreatmentId}");
            }
        }

        public async Task<List<TreatmentOutgoing>> GetTreatments(string OrganisationId, string ServiceproviderId, string CustomerId, bool IsUpcoming)
        {
            DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

            if (!string.IsNullOrWhiteSpace(CustomerId))
            {
                DataValidation.ValidateObjectId(CustomerId, IdType.Customer);
            }
            
            List<Mongo.TreatmentPlan> mongoTreatmentPlans = await treatmentPlanRepository.GetAllTreatmentPlans(OrganisationId, ServiceproviderId, CustomerId);
            
            logger.LogInformation($"Received {mongoTreatmentPlans.Count} treatment plans from db");

            var outgoingTreatments = TreatmentPlanConverter.ConvertToDenormalizedTreatments(mongoTreatmentPlans);
            
            if (IsUpcoming)
            {
                FilterTreatmentPlans(ref outgoingTreatments);
            }

            logger.LogInformation("Converted treatments to outgoing successfully");

            return outgoingTreatments;
        }

        public async Task AddTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:AddTreatmentPlan"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(treatmentPlanIncoming.ServiceProviderId, IdType.ServiceProvider);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.CustomerId, IdType.Customer);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.SourceServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.OrganisationId, IdType.Organisation);

                var customerProfile = await customerRepository.GetCustomerProfile(treatmentPlanIncoming.CustomerId, treatmentPlanIncoming.OrganisationId);
                DataValidation.ValidateObject(customerProfile);

                var serviceProviderProfile = await serviceProviderRepository.GetServiceProviderProfile(treatmentPlanIncoming.ServiceProviderId, treatmentPlanIncoming.OrganisationId);
                DataValidation.ValidateObject(serviceProviderProfile);

                var mongoTreatmentPlan = TreatmentPlanConverter.ConvertToMongoTreatmentPlan(
                    treatmentPlanIncoming,
                    $"{serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}",
                    $"{customerProfile.FirstName} {customerProfile.LastName}"
                    );

                logger.LogInformation("Constructed mongo treatment plan obj");

                await treatmentPlanRepository.Add(mongoTreatmentPlan);

                logger.LogInformation($"Created treatment plan with id: {mongoTreatmentPlan.TreatmentPlanId}");
            }
        }

        public async Task DeleteTreatment(string TreatmentPlanId, string TreatmentId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:DeleteTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);
                DataValidation.ValidateObjectId(TreatmentId, IdType.TreatmentPlan);

                await treatmentPlanRepository.RemoveTreatment(TreatmentPlanId, TreatmentId);

                logger.LogInformation($"Deleted treatment with id: {TreatmentId}");
            }
        }

        public async Task<List<TreatmentPlanOutgoing>> GetTreatmentPlans(string OrganisationId, string ServiceproviderId, string? CustomerId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:GetAllTreatmentPlans"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

                var mongoTreatmentPlans = await treatmentPlanRepository.GetAllTreatmentPlans(OrganisationId, ServiceproviderId);

                logger.LogInformation($"Received {mongoTreatmentPlans.Count} treatment plans from db");

                var outgoingTreatmentPlans = TreatmentPlanConverter.ConvertToOutgoingTreatmentPlanList(mongoTreatmentPlans);

                logger.LogInformation("Converted treatment plans to outgoing successfully");

                return outgoingTreatmentPlans;

            }
        }

        public async Task UpdateTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:UpdateTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);
                DataValidation.ValidateObjectId(treatmentIncoming.TreatmentId, IdType.TreatmentPlan);

                var mongoTreatment = TreatmentPlanConverter.ConvertToMongoTreatment(treatmentIncoming);

                logger.LogInformation("Converted to mongo treatment successfully");

                await treatmentPlanRepository.UpdateTreatment(TreatmentPlanId, mongoTreatment);

                logger.LogInformation($"Updated treatment with id:{treatmentIncoming.TreatmentId} successfully");
            }
        }

        public async Task UpdateTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:UpdateTreatmentPlan"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(treatmentPlanIncoming.TreatmentPlanId, IdType.TreatmentPlan);

                //Here spName and custName are no longer required as we dont update that
                var mongoTreatmentPlan = TreatmentPlanConverter.ConvertToMongoTreatmentPlan(treatmentPlanIncoming, null, null);

                logger.LogInformation("Converted to mongo treatment plan successfully");

                await treatmentPlanRepository.UpsertTreatmentPlan(mongoTreatmentPlan);

                logger.LogInformation($"Updated treatment plan with id:{treatmentPlanIncoming.TreatmentPlanId} successfully");
            }
        }

        private void FilterTreatmentPlans(ref List<ProviderClientOutgoing.TreatmentOutgoing> treatments)
        {
            treatments.RemoveAll(treatment =>
                treatment.Status == Mongo.TreatmentStatus.Cancelled.ToString() ||
                treatment.Status == Mongo.TreatmentStatus.Done.ToString());
        }
    }
}
