using DataModel.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<CustomerProfile>> GetCustomerProfiles(List<string> customerIds, string organisationId)
        {
            var customerObjectIdList = new List<ObjectId>();
            foreach (var custId in customerIds)
            {
                customerObjectIdList.Add(new ObjectId(custId));
            }
            var customerIdFilter = Builders<Customer>.Filter.In(cust => cust.CustomerId, customerObjectIdList);

            var project = Builders<Customer>.Projection.Expression(
                cust => cust.Profiles.Where(profile => profile.OrganisationId == organisationId)
            );

            var result = await this.GetListByFilterAndProject(customerIdFilter, project);

            return result.ToList();
        }

        public async Task AddCustomerProfile(CustomerProfile profile)
        {
            var filter = Builders<Customer>.Filter;

            var nestedFilter = filter.And(
                filter.Eq(sp => sp.CustomerId, new ObjectId(profile.CustomerId)));

            var update = Builders<Customer>.Update.AddToSet(cust => cust.Profiles, profile);

            await this.AddToSet(nestedFilter, update);
        }

        public async Task<Customer> GetCustomerFromPhoneNumber(string phoneNumber)
        {
            var custFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.AuthInfos, authInfo => authInfo.AuthId.Contains(phoneNumber));

            var result = await this.GetSingleByFilter(custFilter);

            return result;
        }

        public async Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            var filter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));

            var project = Builders<Customer>.Projection.Expression(
                cust => cust.Profiles.Where(profile => profile.OrganisationId.Equals(organisationId))
                );

            var customerProfile = await this.GetSingleByFilterAndProject(filter, project);

            return customerProfile;
        }

        public async Task<List<CustomerProfile>> GetCustomersOfOrganisation(string organisationId)
        {
            var organisationFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => profile.OrganisationId == organisationId);

            var project = Builders<Customer>.Projection.Expression(
                cust => cust.Profiles.Where(profile => profile.OrganisationId == organisationId)
                );

            var result = await this.GetListByFilterAndProject(organisationFilter, project);

            return result.ToList();

        }

        public async Task UpdateCustomerProfile(CustomerProfile customerProfile)
        {
            var filter = Builders<Customer>.Filter;

            var nestedFilter = filter.ElemMatch(cust => cust.Profiles, profile => profile.CustomerProfileId == customerProfile.CustomerProfileId);

            var update = Builders<Customer>.Update.Set(cust => cust.CustomerId, new ObjectId(customerProfile.CustomerId));


            if (customerProfile.FirstName != null)
            {
                update = update.Set("Profiles.$.FirstName", customerProfile.FirstName);
            }

            if (customerProfile.LastName != null)
            {
                update = update.Set("Profiles.$.LastName", customerProfile.LastName);
            }

            if (customerProfile.Gender != null)
            {
                update = update.Set("Profiles.$.Gender", customerProfile.Gender);
            }

            if (customerProfile.DateOfBirth != null)
            {
                update = update.Set("Profiles.$.DateOfBirth", customerProfile.DateOfBirth);
            }


            await this.Upsert(nestedFilter, update);
        }
    }
}