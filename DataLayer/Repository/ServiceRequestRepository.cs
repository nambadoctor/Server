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
    public class ServiceRequestRepository : BaseRepository<Customer>, IServiceRequestRepository
    {
        public ServiceRequestRepository(IMongoContext context) : base(context)
        {
        }
        public async Task AddServiceRequest(ServiceRequest serviceRequest)
        {
            var filter = Builders<Customer>.Filter;

            var nestedFilter = filter.Eq(cust => cust.CustomerId, new ObjectId(serviceRequest.CustomerId));

            var update = Builders<Customer>.Update.AddToSet(cust => cust.ServiceRequests, serviceRequest);

            await this.AddToSet(nestedFilter, update);
        }

        public async Task<ServiceRequest> GetServiceRequest(string serviceRequestId)
        {
            var serviceRequestFilter = Builders<Customer>.Filter.ElemMatch(
                        cust => cust.ServiceRequests,
                        serviceRequest => serviceRequest.ServiceRequestId == new ObjectId(serviceRequestId)
                        );

            var project = Builders<Customer>.Projection.Expression(
                cust => cust.ServiceRequests.Where(sr => sr.ServiceRequestId == new ObjectId(serviceRequestId))
                );

            var serviceRequest = await this.GetSingleByFilterAndProject(serviceRequestFilter, project);

            return serviceRequest;
        }

        public async Task UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            var filter = Builders<Customer>.Filter;

            var nestedFilter = filter.ElemMatch(cust => cust.ServiceRequests, sr => sr.ServiceRequestId == serviceRequest.ServiceRequestId);

            var update = Builders<Customer>.Update.Set(cust => cust.CustomerId, new ObjectId(serviceRequest.CustomerId));

            if (serviceRequest.CustomerId != null)
            {
                update = update.Set("ServiceRequests.$.CustomerId", serviceRequest.CustomerId);
            }

            if (serviceRequest.ServiceProviderId != null)
            {
                update = update.Set("ServiceRequests.$.ServiceProviderId", serviceRequest.ServiceProviderId);
            }

            if (serviceRequest.AppointmentId != null)
            {
                update = update.Set("ServiceRequests.$.AppointmentId", serviceRequest.AppointmentId);
            }

            if (serviceRequest.Vitals != null)
            {
                update = update.Set("ServiceRequests.$.Vitals", serviceRequest.Vitals);
            }

            if (serviceRequest.Reports != null)
            {
                update = update.Set("ServiceRequests.$.Reports", serviceRequest.Reports);
            }

            if (serviceRequest.PrescriptionDocuments != null)
            {
                update = update.Set("ServiceRequests.$.PrescriptionDocuments", serviceRequest.PrescriptionDocuments);
            }

            await this.Upsert(nestedFilter, update);
        }
    }
}
