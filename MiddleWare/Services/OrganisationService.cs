using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using DataModel.Shared.Exceptions;

namespace MiddleWare.Services
{
    public class OrganisationService : IOrganisationService
    {
        private IMongoDbDataLayer datalayer;
        private ILogger logger;

        public OrganisationService(IMongoDbDataLayer dataLayer, ILogger<OrganisationService> logger)
        {
            this.datalayer = dataLayer;
            this.logger = logger;
        }

    }
}
