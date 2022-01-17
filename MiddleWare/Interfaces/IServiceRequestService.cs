using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IServiceRequestService
    {
        public Task<ProviderClientOutgoing.VitalsOutgoing> GetVitals(string serviceRequestId);
        public Task UpdateVitals(ProviderClientIncoming.VitalsIncoming vitalsIncoming);
    }
}
