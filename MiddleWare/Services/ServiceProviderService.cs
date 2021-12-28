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
        public async Task<Client.ServiceProvider> GetServiceProviderOrganisationsAsync()
        {
            var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

            if (serviceProvider == null)
            {
                throw new Exception("Serviceprovider not found");
            }

            var organisationList = await datalayer.GetOrganisations(serviceProvider.ServiceProviderId.ToString());

            var defaultOrganisation = organisationList.FirstOrDefault();

            if (defaultOrganisation == null)
            {
                throw new Exception("Serviceprovider not part of any organiosation");
            }

            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                defaultOrganisation
                );

            return clientServiceProvider;
        }
        public async Task<Client.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            var serviceProvider = await datalayer.GetServiceProvider(ServiceProviderId);

            var organisation = await datalayer.GetOrganisation(OrganisationId);

            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                organisation
                );

            return clientServiceProvider;
        }

    }
}
