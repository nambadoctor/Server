using ServerDataModels.ServiceProvider;

namespace NambaMiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<ClientDataModels.ServiceProvider> GetServiceProviderAsync();
    }
}
