using DataModel.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId);
        public Task<Customer> GetCustomerFromPhoneNumber(string phoneNumber);
        public Task<List<CustomerProfile>> GetCustomersOfOrganisation(string organisationId);
        public Task AddCustomerProfile(CustomerProfile profile);
        public Task UpdateCustomerProfile(CustomerProfile profile);
    }
}
