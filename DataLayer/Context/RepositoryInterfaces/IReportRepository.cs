using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IReportRepository : IRepository<ServiceRequest>
    {
        public Task<List<Report>> GetAllReports(string organisationId, string customerId);
        public Task<List<Report>> GetServiceRequestReports(string serviceRequestId);
        public Task AddReport(Report report, string serviceRequestId);
        public Task DeleteReport(string reportId);
    }
}
