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
        private ILogger logger;

        public OrganisationService(ILogger<OrganisationService> logger)
        {
            this.logger = logger;
        }

    }
}
