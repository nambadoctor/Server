using DataModel.Mongo.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface INotificationUserConfigurationRepository : IRepository<NotificationUserConfiguration>
    {
        public Task<NotificationUserConfiguration> GetByServiceProvider(string serviceProviderId, string organisationId);
    }
}
