using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IOrganisationRepository : IRepository<Organisation>
    {
        public Task<List<Organisation>> GetOrganisationsOfServiceProvider(string serviceProviderId);
    }
}
