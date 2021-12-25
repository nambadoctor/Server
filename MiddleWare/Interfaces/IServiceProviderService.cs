using DataModel.Client.Provider;
using Client = DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<Client.ServiceProvider> GetServiceProviderAsync();
    }
}
