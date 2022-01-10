using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface ICustomerService
    {
        public Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfile(string customerId, string organisationId);
        public Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileFromPhoneNumber(string phoneNumber, string organisationId);
        public Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetCustomerProfiles(string organsiationId, List<string> serviceProviderIds);
        public Task<ProviderClientOutgoing.OutgoingCustomerProfile> SetCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile);
        public Task<ProviderClientOutgoing.CustomerWithAppointmentDataOutgoing> SetCustomerProfileWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointmentIncoming);
    }
}
