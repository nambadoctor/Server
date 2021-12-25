using DataLayer;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using Provider = DataModel.Client.Provider;

namespace MiddleWare.Services
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
            var serviceProvider = await datalayer.GetServiceProvider(NambaDoctorContext.NDUserId);
            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                NambaDoctorContext.OrganisationId
                );
            return clientServiceProvider;
        }
    }
}
