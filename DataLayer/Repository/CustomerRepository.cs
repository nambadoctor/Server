using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoContext context) : base(context)
        {
        }

        public Task AddCustomerProfile(CustomerProfile profile)
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetCustomerFromPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerProfile>> GetCustomersOfOrganisation(string organisationId, List<string> serviceProviderIds)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCustomerProfile(CustomerProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}