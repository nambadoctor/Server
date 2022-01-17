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
    public class PrescriptionRepository : BaseRepository<ServiceRequest>, IPrescriptionRepository
    {
        public PrescriptionRepository(IMongoContext context) : base(context)
        {
        }
        public async Task AddPrescriptionDocument(PrescriptionDocument prescriptionDocument, string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.AddToSet(sr => sr.PrescriptionDocuments, prescriptionDocument);

            await this.AddToSet(filter, update);
        }

        public async Task DeletePrescriptionDocument(string serviceRequestId, string prescriptionDocumentId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var update = Builders<ServiceRequest>.Update.PullFilter(
                sr => sr.PrescriptionDocuments,
                doc => doc.PrescriptionDocumentId == new ObjectId(prescriptionDocumentId)
                );

            await this.RemoveFromSet(filter, update);
        }

        public async Task<List<PrescriptionDocument>> GetServiceRequestPrescriptionDocuments(string serviceRequestId)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(sr => sr.ServiceRequestId, new ObjectId(serviceRequestId));

            var project = Builders<ServiceRequest>.Projection.Expression(
                sr => sr.PrescriptionDocuments.Where(_ => true)
                );

            var result = await this.GetListByFilterAndProject(filter, project);

            return result.ToList();
        }
    }
}
