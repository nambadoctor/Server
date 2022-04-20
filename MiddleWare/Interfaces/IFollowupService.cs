using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces;

public interface IFollowupService
{
    public Task SetFollowup(ProviderClientIncoming.FollowupIncoming followupIncoming);
}