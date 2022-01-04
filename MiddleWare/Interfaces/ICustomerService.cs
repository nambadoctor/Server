using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface ICustomerService
    {
        public Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomer(string customerId, string organisationId);
        public Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomers(string organsiationId, List<string> serviceProviderIds);
        public Task<ProviderClientOutgoing.OutgoingCustomerProfile> SetCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile);
        public Task<ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing> SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerAddedData);
    }
}
