using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Services
{
    public class CustomerService : ICustomerService
    {
        private IMongoDbDataLayer datalayer;
        private NambaDoctorContext nambaDoctorContext;
        private INDLogger NDLogger;

        public CustomerService(IMongoDbDataLayer dataLayer, NambaDoctorContext nambaDoctorContext)
        {
            this.datalayer = dataLayer;
            this.nambaDoctorContext = nambaDoctorContext;
            NDLogger = nambaDoctorContext._NDLogger;
        }
        public async Task<CustomerProfile> GetCustomer(string customerId, string organisationId)
        {
            var customer = await datalayer.GetCustomer(customerId);

            var clientCustomer = GetCustomerProfileObject(customer, organisationId);

            return clientCustomer;
        }

        public async Task<List<CustomerProfile>> GetCustomers(string organsiationId, List<string> serviceProviderIds)
        {
            var customers = await datalayer.GetCustomersAddedByOrganisation(organsiationId, serviceProviderIds);

            var clientCustomers = new List<CustomerProfile>();

            foreach (var customer in customers)
            {
                clientCustomers.Add(GetCustomerProfileObject(customer, organsiationId));
            }

            return clientCustomers;
        }

        private CustomerProfile GetCustomerProfileObject(Mongo.Customer customer, string organisationId)
        {
            var customerProfile = (from cp in customer.Profiles
                                   where cp.OrganisationId == organisationId
                                   select cp).FirstOrDefault();

            if (customerProfile == null)
            {
                throw new KeyNotFoundException($"Customer profile id:{customer.CustomerId} not found for organisation id: {organisationId}");
            }

            var clientCustomer = CustomerConverter.ConvertToClientCustomerProfile(customer.CustomerId.ToString(), customerProfile);

            return clientCustomer;
        }
    }
}
