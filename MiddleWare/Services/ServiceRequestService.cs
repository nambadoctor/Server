using DataModel.Client.Provider;
using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public Task<ServiceRequest> GetServiceRequest(string customerId, string appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceRequest> SetServiceRequest(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
