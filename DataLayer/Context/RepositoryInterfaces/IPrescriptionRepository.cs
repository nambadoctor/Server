using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IPrescriptionRepository : IRepository<ServiceRequest>
    {
        public Task<List<PrescriptionDocument>> GetAllPrescriptions(string organisationId, string customerId);
        public Task<List<PrescriptionDocument>> GetServiceRequestPrescriptionDocuments(string serviceRequestId);
        public Task AddPrescriptionDocument(PrescriptionDocument prescriptionDocument, string serviceRequestId);
        public Task DeletePrescriptionDocument(string serviceRequestId, string prescriptionDocumentId);
    }
}
