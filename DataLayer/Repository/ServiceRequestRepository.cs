using DataModel.Mongo;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class ServiceRequestRepository : BaseRepository<Customer>, IServiceRequestRepository
    {
        public ServiceRequestRepository(IMongoContext context) : base(context)
        {
        }
        public Task AddServiceRequest(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceRequest> GetServiceRequest(string appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
