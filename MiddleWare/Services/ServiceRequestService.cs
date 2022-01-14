using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MiddleWare.Interfaces;
using DataModel.Client.Provider.Incoming;

namespace MiddleWare.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public Task<ProviderClientOutgoing.ServiceRequest> GetServiceRequest(string customerId, string appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
