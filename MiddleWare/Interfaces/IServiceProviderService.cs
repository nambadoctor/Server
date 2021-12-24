using Client = ClientDataModels.SP;

namespace NambaMiddleWare.Interfaces
{
    public interface IServiceProviderService
    {
        public Task<Client.ServiceProvider> GetServiceProviderAsync();
    }
}
