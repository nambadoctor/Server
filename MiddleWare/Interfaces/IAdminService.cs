using AdminClientOutgoing = DataModel.Client.Admin.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Mongo;

namespace MiddleWare.Interfaces
{
    public interface IAdminService
    {
        public Task<List<AdminClientOutgoing.OutgoingAdminStat>> GetAdminStats();
    }
}
