using Jobs.Models;
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

    }
}
