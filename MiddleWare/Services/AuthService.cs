using DataLayer;
using DataModel.Mongo;
using DataModel.Shared;
using MiddleWare.Interfaces;

namespace MiddleWare.Services
{
    public class AuthService : IAuthService
    {
        private IMongoDbDataLayer datalayer;
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger NDLogger;

        public AuthService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.datalayer = dataLayer;
            this.nambaDoctorContext = nambaDoctorContext;
            this.NDLogger = nambaDoctorContext._NDLogger;
        }
        public async Task<Customer> GetCustomerFromRegisteredPhoneNumber()
        {
            var customer = await datalayer.GetCustomerFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);
            return customer;
        }

        public async Task<string?> GetDefaultOrganisationId()
        {
            var organisations = await datalayer.GetOrganisations(NambaDoctorContext.NDUserId);
            if (organisations != null && organisations.Count > 0)
            {
                var defaultOrganisation = organisations[0];
                return defaultOrganisation.OrganisationId.ToString();
            }
            else
            {
                return null;
            }
        }

        public async Task<DataModel.Mongo.ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber()
        {
            var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);
            return serviceProvider;
        }
    }
}
