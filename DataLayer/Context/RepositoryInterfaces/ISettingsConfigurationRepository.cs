using System.Threading.Tasks;
using DataModel.Mongo.Configuration;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface ISettingsConfigurationRepository: IRepository<SettingsConfiguration>
    {
        public Task<SettingsConfiguration> GetServiceProviderConfiguration(string ServiceProviderId, string OrganisationId);
    }   
}