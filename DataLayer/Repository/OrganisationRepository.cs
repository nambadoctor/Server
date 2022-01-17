using DataModel.Mongo;
using MongoDB.Driver;
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
        public async Task<List<Organisation>> GetOrganisationsOfServiceProvider(string serviceProviderId)
        {
            var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);

            var result = await this.GetListByFilter(orgFilter);

            return result.ToList();
        }
    }
}
