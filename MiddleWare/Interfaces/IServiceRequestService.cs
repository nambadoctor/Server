using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IServiceRequestService
    {
        public Task<ProviderClientOutgoing.ServiceRequest> GetServiceRequest(string customerId, string appointmentId);
        public Task UpdateServiceRequest(ProviderClientIncoming.ServiceRequest serviceRequest);
    }
}
