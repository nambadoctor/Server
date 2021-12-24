using DataLayer;
using NambaMiddleWare.Interfaces;
using ServerDataModels.Local;
using Client = ClientDataModels.SP;

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
        public async Task<Client.ServiceProvider> GetServiceProviderAsync()
        {
            var serviceProvider = await datalayer.GetServiceProvider(NambaDoctorContext.NDUserId);
            var organisations = await datalayer.GetOrganisations(NambaDoctorContext.NDUserId);

            var clientServiceProvider = new Client.ServiceProvider();

            //Buid client Object

            return clientServiceProvider;
        }
    }
}
