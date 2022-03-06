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

        public async Task<List<ServiceRequest>> GetServiceRequestsMatchingId(List<string> ServiceRequestIds)
        {
            var serviceRequestIdsList = new List<ObjectId>();

            foreach (var serviceProviderId in ServiceRequestIds)
            {
                serviceRequestIdsList.Add(new ObjectId(serviceProviderId));
            }

            var filter = Builders<ServiceRequest>.Filter.In(
                sr => sr.ServiceRequestId,
                serviceRequestIdsList
                );

            var result = await this.GetListByFilter(filter);

            return result.ToList();
        }
    }
}
