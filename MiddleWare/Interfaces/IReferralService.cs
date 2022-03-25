using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IReferralService
    {
        public Task SetReferral(ProviderClientIncoming.ReferralIncoming referralIncoming);
    }
}
