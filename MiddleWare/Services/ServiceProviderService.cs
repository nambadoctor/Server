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
        public async Task<Client.ServiceProviderBasic> GetServiceProviderOrganisationsAsync()
        {
            var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

            if (serviceProvider == null)
            {
                throw new Exception($"Serviceprovider not found with phone: {NambaDoctorContext.PhoneNumber}");
            }

            var organisationList = await datalayer.GetOrganisations(serviceProvider.ServiceProviderId.ToString());

            var defaultOrganisation = organisationList.FirstOrDefault();

            if (defaultOrganisation == null)
            {
                throw new Exception("Serviceprovider not part of any organisation in {}");
            }

            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProviderBasic(
                serviceProvider,
                organisationList,
                defaultOrganisation
                );

            return clientServiceProvider;
        }
        public async Task<Client.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            var serviceProvider = await datalayer.GetServiceProvider(ServiceProviderId);

            if (serviceProvider == null)
            {
                throw new Exception($"Serviceprovider not found with id: {ServiceProviderId}");
            }

            var organisation = await datalayer.GetOrganisation(OrganisationId);

            if (organisation == null)
            {
                throw new Exception($"Organisation not found with id: {OrganisationId}");
            }

            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                organisation
                );

            return clientServiceProvider;
        }

    }
}
