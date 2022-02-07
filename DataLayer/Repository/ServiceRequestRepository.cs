using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class ServiceRequestRepository : BaseRepository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<ServiceRequest>> GetServiceRequests(List<string> serviceRequestIds)
        {
            var serviceRequestObjectIdList = new List<ObjectId>();
            foreach (var serviceRequestId in serviceRequestIds)
            {
                serviceRequestObjectIdList.Add(new ObjectId(serviceRequestId));
            }
            var serviceRequestFilter = Builders<ServiceRequest>.Filter.In(sr => sr.ServiceRequestId, serviceRequestObjectIdList);

            var result = await this.GetListByFilter(serviceRequestFilter);

            return result.ToList();
        }
    }
}
