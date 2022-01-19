using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class ReportRepository : BaseRepository<ServiceRequest>, IReportRepository
    {
        public ReportRepository(IMongoContext context) : base(context)
        {
        }
        public async Task AddReport(Report report, string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.AddToSet(sr => sr.Reports, report);

            await this.AddToSet(filter, update);
        }

        public async Task DeleteReport(string serviceRequestId, string reportId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.PullFilter(
                sr => sr.Reports,
                report => report.ReportId == new ObjectId(reportId)
                );

            await this.RemoveFromSet(filter, update);
        }

        public async Task<List<Report>> GetServiceRequestReports(string serviceRequestId)
        {
            try
            {
                var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

                var project = Builders<ServiceRequest>.Projection.Expression(
                    sr => sr.Reports.Where(_ => true)
                    );

                var result = await this.GetListByFilterAndProject(filter, project);

                return result.ToList();
            } catch (Exception e)
            {
                return null;
            }
        }
    }
}
