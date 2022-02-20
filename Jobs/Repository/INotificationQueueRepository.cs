using Jobs.Models;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Repository
{
    public interface INotificationQueueRepository : IRepository<NotificationQueue>
    {
    }
}
