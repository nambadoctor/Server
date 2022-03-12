using DataModel.Mongo.Notification;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class NotificationQueueRepository : BaseRepository<NotificationQueue>, INotificationQueueRepository
    {
        public NotificationQueueRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<NotificationQueue>> GetPending()
        {
            var filter = Builders<NotificationQueue>.Filter.Lt(nq => nq.ToBeNotifiedTime, DateTime.UtcNow);

            var pendingQueue = await this.GetListByFilter(filter);

            return pendingQueue.ToList();
        }

        public async Task RemoveAllMatchingIdList(string appointmentId)
        {

            var filter = Builders<NotificationQueue>.Filter.Eq(nq => nq.AppointmentId, appointmentId);

            await this.RemoveWithFilter(filter);
        }
    }
}
