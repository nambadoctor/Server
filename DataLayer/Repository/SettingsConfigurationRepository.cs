using System;
using DataModel.Mongo.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class SettingsConfigurationRepository:  BaseRepository<SettingsConfiguration>, ISettingsConfigurationRepository
    {

        public SettingsConfigurationRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<SettingsConfiguration> GetServiceProviderConfiguration(string ServiceProviderId, string OrganisationId)
        {
            var serviceProviderFilter = Builders<SettingsConfiguration>.Filter.Eq(config => config.ServiceProviderId, ServiceProviderId);
            
            var orgFilter = Builders<SettingsConfiguration>.Filter.Eq(config => config.OrganisationId, OrganisationId);

            var config = await this.GetSingleByFilter(serviceProviderFilter & orgFilter);

            return config;
        }
    }   
}