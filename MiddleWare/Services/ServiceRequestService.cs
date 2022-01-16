using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Interfaces;
using DataModel.Client.Provider.Incoming;
using MongoDB.GenericRepository.Interfaces;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Utils;

namespace MiddleWare.Services
{
    public class ServiceRequestService : IServiceRequestService
    {

        private IVitalsRepository vitalsRepository;
        private ILogger logger;

        public ServiceRequestService(IVitalsRepository vitalsRepository, ILogger<ServiceProviderService> logger)
        {
            this.vitalsRepository = vitalsRepository;
            this.logger = logger;
        }

        public async Task<ProviderClientOutgoing.VitalsOutgoing> GetVitals(string serviceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceRequestService:GetVitals"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(serviceRequestId, IdType.ServiceRequest);

                var vitals = await vitalsRepository.GetServiceRequestVitals(serviceRequestId);

                var vitalsToReturn = ServiceRequestConverter.ConvertToClientOutgoingVitals(vitals, serviceRequestId);

                return vitalsToReturn;
            }
        }

        public async Task UpdateVitals(VitalsIncoming vitalsIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceRequestService:UpdateVitals"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(vitalsIncoming.ServiceRequestId, IdType.ServiceRequest);

                var vitals = ServiceRequestConverter.ConvertToMongoVitals(vitalsIncoming);

                await vitalsRepository.UpdateVitals(vitals, vitalsIncoming.ServiceRequestId);
            }
        }
    }
}
