using DataLayer;
using NambaMiddleWare.Interfaces;
using ServerDataModels.Local;
using ServerDataModels.ServiceProvider;

namespace NambaMiddleWare.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private IMongoDbDataLayer datalayer;
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger NDLogger;

        public ServiceProviderService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.datalayer = dataLayer;
            this.NDLogger = nambaDoctorContext._NDLogger;
        }
        public async Task<ClientDataModels.ServiceProvider> GetServiceProviderAsync()
        {
            var serviceProvider = await datalayer.GetServiceProvider(NambaDoctorContext.NDUserId);
            var organisations = await datalayer.GetOrganisations(NambaDoctorContext.NDUserId);

            var clientServiceProvider = new ClientDataModels.ServiceProvider();

            clientServiceProvider.ServiceProviderId = serviceProvider.ServiceProviderId.ToString();
            clientServiceProvider.Organisations = organisations;
            clientServiceProvider.Profiles = serviceProvider.Profiles;

            return clientServiceProvider;
        }
    }
}
