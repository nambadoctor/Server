using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IVitalsRepository : IRepository<ServiceRequest>
    {
        public Task<Vitals> GetServiceRequestVitals(string serviceRequestId);
        public Task UpdateVitals(Vitals vitals, string serviceRequestId);
    }
}
