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
            var organisationList = await datalayer.GetOrganisations(NambaDoctorContext.NDUserId);

            var clientOrganisationList = OrganisationConverter.ConvertToClientOrganisationList(organisationList);

            return clientOrganisationList;
        }
    }
}
