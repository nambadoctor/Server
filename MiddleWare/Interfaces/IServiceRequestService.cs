using Client = DataModel.Client.Provider;

namespace MiddleWare.Interfaces
{
    public interface IServiceRequestService
    {
        public Task<Client.ServiceRequest> GetServiceRequest(string customerId, string appointmentId);
        public Task<Client.ServiceRequest> SetServiceRequest(Client.ServiceRequest serviceRequest);
    }
}
