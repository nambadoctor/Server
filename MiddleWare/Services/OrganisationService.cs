using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;


namespace MiddleWare.Services
{
    public class OrganisationService : IOrganisationService
    {
        private IMongoDbDataLayer datalayer;
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger NDLogger;

        public OrganisationService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.datalayer = dataLayer;
            this.nambaDoctorContext = nambaDoctorContext;
            NDLogger = nambaDoctorContext._NDLogger;
        }

        public async Task<Organisation> GetOrganisationAsync(string OrganisationId)
        {
            var organisation = await datalayer.GetOrganisation(OrganisationId);

            if (organisation == null)
            {
                throw new KeyNotFoundException($"Organisation not found for id: {OrganisationId}");
            }

            var listOfServiceProviderIds = new List<string>();

            if (organisation.Members != null)
            {
                foreach (var member in organisation.Members)
                {
                    listOfServiceProviderIds.Add(member.ServiceProviderId);
                }
            }

            if (listOfServiceProviderIds.Count == 0)
            {
                throw new KeyNotFoundException($"No members for this organisation id:{OrganisationId}");
            }

            var serviceProviders = await datalayer.GetServiceProviders(listOfServiceProviderIds);

            var clientOrganisation = OrganisationConverter.ConvertToClientOrganisation(
                organisation, serviceProviders);

            return clientOrganisation;
        }
    }
}
