using DataLayer;
using DataModel.Shared;
using NambaMiddleWare.Interfaces;
using Provider = DataModel.Client.Provider;

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
        public async Task<Provider.ServiceProvider> GetServiceProviderAsync()
        {
            /*
            var serviceProvider = await datalayer.GetServiceProvider(NambaDoctorContext.NDUserId);
            var organisations = await datalayer.GetOrganisations(NambaDoctorContext.NDUserId);
            */
            var clientServiceProvider = new Provider.ServiceProvider();

            //Buid client Object

            return clientServiceProvider;
        }
    }
}
