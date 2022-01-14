using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IServiceRequestRepository : IRepository<Customer>
    {
        public Task<ServiceRequest> GetServiceRequest(string serviceRequestId);
        public Task AddServiceRequest(ServiceRequest serviceRequest);
        public Task UpdateServiceRequest(ServiceRequest serviceRequest);
    }
}
