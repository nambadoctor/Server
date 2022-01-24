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
    }
}
