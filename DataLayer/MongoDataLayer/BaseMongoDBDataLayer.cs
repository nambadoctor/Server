using MongoDB.Bson;
using MongoDB.Driver;
using ServerDataModels.Customer;
using ServerDataModels.Local;
using ServerDataModels.Organisation;
using ServerDataModels.Other;
using ServerDataModels.ServiceProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public INDLogger _NDLogger;

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
            this._NDLogger = nambaDoctorContext._NDLogger;
        }

        public async Task<ServiceProvider> GetServiceProvider(string serviceProviderId)
        {
            _NDLogger.LogEvent("Start GetServiceProvider");
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            _NDLogger.LogEvent("End GetServiceProvider");
            return result;
        }

        public async Task<List<Organisation>> GetOrganisations(string serviceProviderId)
        {
            _NDLogger.LogEvent("Start GetOrganisations");
            var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);
            var result = await this.organisationCollection.Find(orgFilter).ToListAsync();
            _NDLogger.LogEvent("End GetOrganisations");
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
