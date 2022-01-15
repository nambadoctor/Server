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

        private IServiceRequestRepository serviceRequestRepository;
        private ILogger logger;

        public ServiceRequestService(IServiceRequestRepository serviceRequestRepository, ILogger<ServiceProviderService> logger)
        {
            this.serviceRequestRepository = serviceRequestRepository;
            this.logger = logger;
        }

        public async Task<ProviderClientOutgoing.VitalsOutgoing> GetVitals(string serviceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceRequestService:GetVitals"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(serviceRequestId, IdType.ServiceRequest);

                var serviceRequest = await serviceRequestRepository.GetServiceRequest(serviceRequestId);

                var vitals = ServiceRequestConverter.ConvertToClientOutgoingVitals(serviceRequest.Vitals, serviceRequestId);

                return vitals;
            }
        }

        public async Task UpdateVitals(VitalsIncoming vitalsIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceRequestService:UpdateVitals"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(vitalsIncoming.ServiceRequestId, IdType.ServiceRequest);

                var serviceRequestFromDb = await serviceRequestRepository.GetServiceRequest(vitalsIncoming.ServiceRequestId);

                DataValidation.ValidateObject(serviceRequestFromDb);

                //Construct service request with vitals and ID
                var serviceRequest = new Mongo.ServiceRequest();
                serviceRequest.CustomerId = serviceRequestFromDb.CustomerId;
                serviceRequest.ServiceRequestId = new MongoDB.Bson.ObjectId(vitalsIncoming.ServiceRequestId);
                serviceRequest.Vitals = ServiceRequestConverter.ConvertToMongoVitals(vitalsIncoming);

                await serviceRequestRepository.UpdateServiceRequest(serviceRequest);
            }
        }
    }
}
