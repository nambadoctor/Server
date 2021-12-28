using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using Client = DataModel.Client.Provider;

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
        public async Task<List<Client.OrganisationBasic>> GetServiceProviderOrganisations()
        {
            var serviceProvider = await datalayer.GetServiceProvider(NambaDoctorContext.NDUserId);
            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                NambaDoctorContext.OrganisationId
                );
            return null;
        }
        public async Task<Client.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
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
