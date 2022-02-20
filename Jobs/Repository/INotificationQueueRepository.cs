using Jobs.Models;
using MongoDB.GenericRepository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jobs.Repository
{
    public interface INotificationQueueRepository : IRepository<NotificationQueue>
    {
        public Task<List<NotificationQueue>> GetPending();
        public Task RemoveAllMatchingId(string appointmentId);
        public Task RemoveAllMatchingIdList(List<string> notificationQIds);
    }
}
