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

        public async Task<List<Organisation>> GetOrganisationsAsync()
        {
            var sp = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

            var organisationList = await datalayer.GetOrganisations(sp.ServiceProviderId.ToString());

            var listOfServiceProviderIds = new List<string>();

            foreach (var organisation in organisationList)
            {
                foreach (var member in organisation.Members)
                {
                    listOfServiceProviderIds.Add(member.ServiceProviderId);
                }
            }

            var serviceProviders = await datalayer.GetServiceProviders(listOfServiceProviderIds);

            var clientOrganisationList = OrganisationConverter.ConvertToClientOrganisationList(
                organisationList, serviceProviders);

            return clientOrganisationList;
        }
    }
}
