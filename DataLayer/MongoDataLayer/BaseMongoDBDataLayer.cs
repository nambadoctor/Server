using MongoDB.Bson;
using MongoDB.Driver;
using DataModel.Mongo;

using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Shared;

namespace DataLayer
{
    public class BaseMongoDBDataLayer : IMongoDbDataLayer
    {
        public IMongoDatabase dbInstance;
        public IMongoCollection<ServiceProvider> serviceProviderCollection;
        public IMongoCollection<Customer> customerCollection;
        public IMongoCollection<AutofillMedicine> autoFillMedicineCollection;

        public IMongoCollection<Category> serviceProviderCategoriesCollection;
        public IMongoCollection<BlockedUser> blockedUsersCollection;
        public IMongoCollection<TestUser> testUserCollection;

        public IMongoCollection<Organisation> organisationCollection;
        public IMongoCollection<ServiceProviderCreatedTemplate> serviceProviderCreatedTemplatesCollection;


        public NambaDoctorContext _nambaDoctorContext;

        public IMongoDatabase DbInstance
        {
            get
            {
                return dbInstance;
            }
        }

        public BaseMongoDBDataLayer(NambaDoctorContext nambaDoctorContext)
        {
            var mongoDBInstance = new MongoDBInstance();

            dbInstance = mongoDBInstance.GetMongoDB();

            this.serviceProviderCollection = dbInstance.GetCollection<ServiceProvider>(ConnectionConfiguration.ServiceProvideCollection);
            this.customerCollection = dbInstance.GetCollection<Customer>(ConnectionConfiguration.CustomerCollection);
            this.autoFillMedicineCollection = dbInstance.GetCollection<AutofillMedicine>(ConnectionConfiguration.MedicineAutoFillCollection);
            this.serviceProviderCategoriesCollection = dbInstance.GetCollection<Category>(ConnectionConfiguration.ServiceProviderCategoriesCollection);
            this.blockedUsersCollection = dbInstance.GetCollection<BlockedUser>(ConnectionConfiguration.BlockedUsersCollection);
            this.testUserCollection = dbInstance.GetCollection<TestUser>(ConnectionConfiguration.TestUsersCollection);
            this.organisationCollection = dbInstance.GetCollection<Organisation>(ConnectionConfiguration.OrganisationCollection);
            this.serviceProviderCreatedTemplatesCollection = dbInstance.GetCollection<ServiceProviderCreatedTemplate>(ConnectionConfiguration.ServiceProviderCreatedTemplatesCollection);

            this._nambaDoctorContext = nambaDoctorContext;
        }

        public async Task<ServiceProvider> GetServiceProvider(string serviceProviderId)
        {
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<ServiceProvider>> GetServiceProviders(List<string> serviceProviderIds)
        {
            var objectIdList = new List<ObjectId>();
            foreach (var spId in serviceProviderIds)
            {
                objectIdList.Add(new ObjectId(spId));
            }
            var filter = Builders<ServiceProvider>.Filter.In(sp => sp.ServiceProviderId, objectIdList);
            var result = await this.serviceProviderCollection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<Organisation> GetOrganisation(string organisationId)
        {
            var orgFilter = Builders<Organisation>.Filter.Eq(org => org.OrganisationId, new ObjectId(organisationId));
            var result = await this.organisationCollection.Find(orgFilter).FirstOrDefaultAsync();
            return result;
        }
        public async Task<List<Organisation>> GetOrganisations(string serviceProviderId)
        {
            var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);
            var result = await this.organisationCollection.Find(orgFilter).ToListAsync();
            return result;
        }

        public async Task<string> GetUserTypeFromRegisteredPhoneNumber(string phoneNumber)
        {
            var sp = await GetServiceProviderFromRegisteredPhoneNumber(phoneNumber);
            var cust = await GetCustomerFromRegisteredPhoneNumber(phoneNumber);
            if (cust != null)
            {
                return $"Customer,{cust.CustomerId}";
            }
            if (sp != null)
            {
                return $"ServiceProvider,{sp.ServiceProviderId}";
            }

            return "NotRegistered";

        }

        public async Task<Customer> GetCustomerFromRegisteredPhoneNumber(string phoneNumber)
        {
            var custFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
            var result = await this.customerCollection.Find(custFilter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber)
        {
            var spFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            return result;
        }

    }
}
