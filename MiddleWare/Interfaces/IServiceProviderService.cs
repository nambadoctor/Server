using DataModel.Client.Provider;
using Client = DataModel.Client.Provider;

namespace NambaMiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<Client.ServiceProvider> GetServiceProviderAsync();
    }
}
