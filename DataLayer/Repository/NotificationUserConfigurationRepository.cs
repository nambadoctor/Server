using DataModel.Mongo;
using DataModel.Mongo.Notification;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class NotificationUserConfigurationRepository : BaseRepository<NotificationUserConfiguration>, INotificationUserConfigurationRepository
    {
        public NotificationUserConfigurationRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<NotificationUserConfiguration> GetByServiceProvider(string serviceProviderId, string organisationId)
        {
            var organisationFilter = Builders<NotificationUserConfiguration>.Filter.Eq(sr => sr.OrganisationId, organisationId);

            var spFilter = Builders<NotificationUserConfiguration>.Filter.Eq(sr => sr.ServiceProviderId, serviceProviderId);

            var combinedFilter = organisationFilter & spFilter;

            var result = await this.GetSingleByFilter(filter: combinedFilter);

            return result;
        }
    }
}
