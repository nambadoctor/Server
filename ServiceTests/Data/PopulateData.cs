using DataModel.Mongo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.Data
{
    [TestClass()]
    public class PopulateData
    {
        private IMongoDatabase database;

        private IMongoCollection<ServiceProvider> serviceProviderCollection;
        private IMongoCollection<Customer> customerCollection;
        private IMongoCollection<Organisation> organisationCollection;

        private ObjectId SpId1;
        private ObjectId SpId2;
        private ObjectId OrgId1;
        private ObjectId CustId1;
        private ObjectId CustId2;
        private void SetClient()
        {
            var mongoClient = new MongoClient("***PROD STRING***");
            database = mongoClient.GetDatabase("NambaDoctorDbPpe");

            this.serviceProviderCollection = database.GetCollection<ServiceProvider>("ServiceProviders");
            this.customerCollection = database.GetCollection<Customer>("Customer");
            this.organisationCollection = database.GetCollection<Organisation>("Organisations");

        }

        private void SetIds()
        {
            SpId1 = ObjectId.GenerateNewId();
            SpId2 = ObjectId.GenerateNewId();
            OrgId1 = ObjectId.GenerateNewId();
            CustId1 = ObjectId.GenerateNewId();
            CustId2 = ObjectId.GenerateNewId();
        }

        private async Task SetServiceProviders()
        {
            var sp1 = new ServiceProvider();
            sp1.ServiceProviderId = SpId1;
            sp1.AuthInfos = new List<AuthInfo>();
            sp1.AuthInfos.Add(new AuthInfo
            {
                AuthId = "+919999999999",
                AuthType = "PhoneNumber"
            });
            sp1.Profiles = new List<ServiceProviderProfile>();
            sp1.Profiles.Add(new ServiceProviderProfile
            {
                ServiceProviderId = sp1.ServiceProviderId.ToString(),
                FirstName = "Jacob",
                OrganisationId = OrgId1.ToString(),
                ServiceProviderType = "Secretary",
                LastName = "LastName",
                Gender = "Male"
            });
            sp1.Availabilities = new List<ServiceProviderAvailability>();
            sp1.Availabilities.Add(new ServiceProviderAvailability
            {
                AppointmentType = AppointmentType.Inperson,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                OrganisationId = OrgId1.ToString(),
                PaymentType = PaymentType.PostPay
            });

            var sp2 = new ServiceProvider();
            sp2.ServiceProviderId = SpId2;
            sp2.AuthInfos = new List<AuthInfo>();
            sp2.AuthInfos.Add(new AuthInfo
            {
                AuthId = "+911234567890",
                AuthType = "PhoneNumber"
            });
            sp2.Profiles = new List<ServiceProviderProfile>();
            sp2.Profiles.Add(new ServiceProviderProfile
            {
                ServiceProviderId = sp1.ServiceProviderId.ToString(),
                FirstName = "Surya",
                OrganisationId = OrgId1.ToString(),
                ServiceProviderType = "Doctor",
                LastName = "LastName",
                Gender = "Male"
            });
            sp2.Availabilities = new List<ServiceProviderAvailability>();
            sp2.Availabilities.Add(new ServiceProviderAvailability
            {
                AppointmentType = AppointmentType.Online,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                OrganisationId = OrgId1.ToString(),
                PaymentType = PaymentType.PostPay
            });

            await this.serviceProviderCollection.InsertOneAsync(sp1);
            await this.serviceProviderCollection.InsertOneAsync(sp2);
        }

        private async Task SetOrganisation()
        {
            var org1 = new Organisation();
            org1.OrganisationId = OrgId1;
            org1.Name = "Random org";
            org1.PhoneNumbers = new List<PhoneNumber>();
            org1.EmailAddresses = new List<string>();
            org1.Members = new List<Member>();
            org1.Members.Add(new Member { ServiceProviderId = SpId1.ToString(), Role = "Admin" });
            org1.Members.Add(new Member { ServiceProviderId = SpId2.ToString(), Role = "Doctor" });

            await this.organisationCollection.InsertOneAsync(org1);

        }

        private async Task SetCustomers()
        {
            var cust1 = new Customer();
            cust1.CustomerId = CustId1;
            cust1.AuthInfos = new List<AuthInfo>();
            cust1.Profiles = new List<CustomerProfile>();
            cust1.Profiles.Add(new CustomerProfile
            {
                CustomerId = CustId1.ToString(),
                FirstName = "Joshua",
                Gender = "Male",
                LastName = "Abraham",
                Languages = new List<string>(),
                OrganisationId = OrgId1.ToString(),
                ServiceProviderId = SpId1.ToString()
            });
            cust1.AuthInfos.Add(new AuthInfo { AuthId = "+917777777777", AuthType = "PhoneNumber" });

            var cust2 = new Customer();
            cust2.CustomerId = CustId2;
            cust2.AuthInfos = new List<AuthInfo>();
            cust2.Profiles = new List<CustomerProfile>();
            cust2.Profiles.Add(new CustomerProfile
            {
                CustomerId = CustId2.ToString(),
                FirstName = "Nithin",
                Gender = "Male",
                LastName = "Manivannan",
                Languages = new List<string>(),
                OrganisationId = OrgId1.ToString(),
                ServiceProviderId = SpId2.ToString()
            });
            cust2.AuthInfos.Add(new AuthInfo { AuthId = "+918888888888", AuthType = "PhoneNumber" });

            await this.customerCollection.InsertOneAsync(cust1);
            await this.customerCollection.InsertOneAsync(cust2);
        }

        [TestMethod()]
        public async Task PopulateBasicData()
        {
            SetClient();
            SetIds();
            await SetServiceProviders();
            await SetOrganisation();
            await SetCustomers();
        }
    }
}
