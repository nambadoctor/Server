using DataLayer;
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

        public ServiceProviderService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.datalayer = dataLayer;
        }
        public async Task<Client.ServiceProviderBasic> GetServiceProviderOrganisationsAsync()
        {
            var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

            if (serviceProvider == null)
            {
                throw new KeyNotFoundException($"Serviceprovider not found with phone: {NambaDoctorContext.PhoneNumber}");
            }

            var organisationList = await datalayer.GetOrganisations(serviceProvider.ServiceProviderId.ToString());

            var defaultOrganisation = organisationList.FirstOrDefault();

            if (defaultOrganisation == null)
            {
                throw new KeyNotFoundException($"Serviceprovider :{serviceProvider.ServiceProviderId},{NambaDoctorContext.PhoneNumber} not part of any organisation");
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
                throw new KeyNotFoundException($"Serviceprovider not found with id: {ServiceProviderId}");
            }

            var organisation = await datalayer.GetOrganisation(OrganisationId);

            if (organisation == null)
            {
                throw new KeyNotFoundException($"Organisation not found with id: {OrganisationId}");
            }

            //Find role in org
            var role = organisation.Members.Find(member => member.ServiceProviderId == serviceProvider.ServiceProviderId.ToString());
            if (role == null)
            {
                throw new KeyNotFoundException($"No role found for this service provider({serviceProvider.ServiceProviderId}) in organisation with id: {OrganisationId}");
            }

            //Find profile to map
            var spProfile = serviceProvider.Profiles.Find(profile => profile.OrganisationId == organisation.OrganisationId.ToString());
            if (spProfile == null)
            {
                throw new KeyNotFoundException($"No profile for service provider: ({serviceProvider.ServiceProviderId}) found with mathcing organisation id : {OrganisationId}");
            }
            //Buid client Object
            var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                serviceProvider,
                organisation,
                role,
                spProfile
                );

            return clientServiceProvider;
        }

    }
}
