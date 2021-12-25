using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using NambaMiddleWare.Interfaces;


namespace NambaMiddleWare.Services
{
    public class OrganisationService : IOrganisationService
    {
        private IMongoDbDataLayer _datalayer;
        private NambaDoctorContext _nambaDoctorContext;
        private INDLogger _NDLogger;

        public OrganisationService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            _datalayer = dataLayer;
            _nambaDoctorContext = nambaDoctorContext;
            _NDLogger = _nambaDoctorContext._NDLogger;
        }
        public async Task<List<Organisation>> GetOrganisationsAsync()
        {
            var organisationList = await _datalayer.GetOrganisations(NambaDoctorContext.NDUserId);
            return null;
           // return organisationList;
        }
    }
}
