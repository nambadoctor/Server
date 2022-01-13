using DataModel.Mongo;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class OrganisationRepository : BaseRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(IMongoContext context) : base(context)
        {
        }
        public Task<List<Organisation>> GetOrganisationsOfServiceProvider(string serviceProviderId)
        {
            throw new NotImplementedException();
        }
    }
}
