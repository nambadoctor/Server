using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public Task<ProviderClientOutgoing.ServiceRequest> GetServiceRequest(string customerId, string appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<ProviderClientOutgoing.ServiceRequest> SetServiceRequest(ProviderClientIncoming.ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
