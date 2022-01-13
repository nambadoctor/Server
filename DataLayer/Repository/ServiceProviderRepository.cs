using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class ServiceProviderRepository : BaseRepository<ServiceProvider>, IServiceProviderRepository
    {
        public ServiceProviderRepository(IMongoContext context) : base(context)
        {
        }

        public Task<ServiceProvider> GetServiceProviderFromPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceProviderProfile> GetServiceProviderProfile(string organisationId, string serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(string organisationId)
        {
            throw new NotImplementedException();
        }
    }
}