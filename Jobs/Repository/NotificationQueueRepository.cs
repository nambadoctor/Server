using Jobs.Models;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Repository
{
    public class NotificationQueueRepository : BaseRepository<NotificationQueue>, INotificationQueueRepository
    {
        public NotificationQueueRepository(IMongoContext context) : base(context)
        {
        }

        public async Task RemoveAllMatchingId(string appointmentId)
        {
            var filter = Builders<NotificationQueue>.Filter.Eq(nq => nq.AppointmentId, appointmentId);

            await this.RemoveWithFilter(filter);
        }
    }
}
