using DataModel.Mongo;
using DataModel.Shared;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
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

        private  ILogger logger;

        public IMongoDatabase DbInstance
        {
            get
            {
                return dbInstance;
            }
        }

        public BaseMongoDBDataLayer(NambaDoctorContext nambaDoctorContext, ILogger<BaseMongoDBDataLayer> logger)
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

            this.logger = logger;
        }

        #region ServiceProvider

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProvider(string serviceProviderId)
        {
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            return result;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderFromRegisteredPhoneNumber"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                var spFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
                
                logger.LogInformation("Calling DB to get Service provider from registerd phone number");
                var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
                
                logger.LogInformation("Calling DB to get Service provider from registerd phone number");

                return result;
            }
        }

        /// <inheritdoc />
        public async Task<ServiceProviderProfile> GetServiceProviderProfile(string serviceProviderId, string organisationId)
        {
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

            var project = Builders<ServiceProvider>.Projection.ElemMatch(
                sp => sp.Profiles,
                profile => profile.OrganisationId == organisationId
                );

            var result = await this.serviceProviderCollection.Find(spFilter).Project<ServiceProviderProfile>(project).FirstOrDefaultAsync();

            return result;
        }

        /// <inheritdoc />
        public async Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(List<string> serviceProviderIds, string organisationId)
        {
            var serviceProviderIdList = new List<ObjectId>();

            foreach (var serviceProviderId in serviceProviderIds)
            {
                serviceProviderIdList.Add(new ObjectId(serviceProviderId));
            }

            var filter = Builders<ServiceProvider>.Filter.In(sp => sp.ServiceProviderId, serviceProviderIdList);

            var project = Builders<ServiceProvider>.Projection.ElemMatch(
                sp => sp.Profiles,
                profile => profile.OrganisationId == organisationId
                );

            var profiles = await this.serviceProviderCollection.Find(filter).Project<ServiceProviderProfile>(project).ToListAsync();

            return profiles;
        }

        /// <inheritdoc />
        public async Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            var serviceProviderFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

            var project = Builders<ServiceProvider>.Projection.ElemMatch(
                sp => sp.Appointments,
                appointment => appointment.AppointmentId == new ObjectId(appointmentId)
                );

            var appointment = await this.serviceProviderCollection.Find(serviceProviderFilter).Project<Appointment>(project).FirstOrDefaultAsync();


            return appointment;
        }

        /// <inheritdoc />
        public async Task<List<Appointment>> GetAppointmentsForServiceProvider(string organisationId, List<string> serviceProviderIds)
        {
            var serviceProviderIdList = new List<ObjectId>();
            foreach (var spId in serviceProviderIds)
            {
                serviceProviderIdList.Add(new ObjectId(spId));
            }

            //Filter according to org and service provider

            var organisationAppointmentFilter = Builders<ServiceProvider>.Filter.ElemMatch(
                sp => sp.Appointments,
                appointment => appointment.OrganisationId == organisationId
                );

            var serviceProviderFilter = Builders<ServiceProvider>.Filter.In(
                sp => sp.ServiceProviderId,
                serviceProviderIdList
                );

            FilterDefinition<ServiceProvider> combinedFilter;
            if (serviceProviderIds.Count == 0)
            {
                combinedFilter = organisationAppointmentFilter;
            }
            else
            {
                combinedFilter = organisationAppointmentFilter & serviceProviderFilter;
            }

            var project = Builders<ServiceProvider>.Projection.ElemMatch(
                sp => sp.Appointments,
                appointment => appointment.OrganisationId == organisationId);

            var appointments = await this.serviceProviderCollection.Find(combinedFilter).Project<Appointment>(project).ToListAsync();

            return appointments;
        }

        #endregion ServiceProvider

        #region Customer

        /// <inheritdoc />
        public async Task<Customer> GetCustomerFromRegisteredPhoneNumber(string phoneNumber)
        {
            var custFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
            var result = await this.customerCollection.Find(custFilter).FirstOrDefaultAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<Customer> GetCustomer(string customerId)
        {
            var custFilter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));
            var result = await this.customerCollection.Find(custFilter).FirstOrDefaultAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Customer>> GetCustomers(List<string> customerIds)
        {
            var objectIdList = new List<ObjectId>();
            foreach (var custId in customerIds)
            {
                objectIdList.Add(new ObjectId(custId));
            }
            var filter = Builders<Customer>.Filter.In(cust => cust.CustomerId, objectIdList);
            var result = await this.customerCollection.Find(filter).ToListAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Customer>> GetCustomersAddedByOrganisation(string organisationId, List<string> serviceProviderIds)
        {
            var organisationFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => profile.OrganisationId == organisationId);

            var serviceProviderFilter = Builders<Customer>.Filter.ElemMatch(
                cust => cust.Profiles,
                profile => serviceProviderIds.Contains(profile.ServiceProviderId) //Dont know if this will work
                );

            FilterDefinition<Customer> combinedFilter;
            if (serviceProviderIds.Count == 0)
            {
                combinedFilter = organisationFilter;
            }
            else
            {
                combinedFilter = organisationFilter & serviceProviderFilter;
            }

            var result = await this.customerCollection.Find(combinedFilter).ToListAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            var filter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.Profiles,
                profile => profile.OrganisationId.Equals(organisationId));

            var profile = await this.customerCollection.Find(filter).Project<CustomerProfile>(project).FirstOrDefaultAsync();

            return profile;
        }

        /// <inheritdoc />
        public async Task<List<CustomerProfile>> GetCustomerProfiles(List<string> customerIds, string organisationId)
        {

            var customerIdList = new List<ObjectId>();

            foreach (var customerId in customerIds)
            {
                customerIdList.Add(new ObjectId(customerId));
            }

            var filter = Builders<Customer>.Filter.In(cust => cust.CustomerId, customerIdList);

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.Profiles,
                profile => profile.OrganisationId.Equals(organisationId));

            var profiles = await this.customerCollection.Find(filter).Project<CustomerProfile>(project).ToListAsync();

            return profiles;
        }

        /// <inheritdoc />
        public async Task<List<CustomerProfile>> GetCustomerProfilesAddedByOrganisation(string organisationId, List<string> serviceProviderIds)
        {
            var organisationFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => profile.OrganisationId == organisationId);

            var serviceProviderFilter = Builders<Customer>.Filter.ElemMatch(
                cust => cust.Profiles,
                profile => serviceProviderIds.Contains(profile.ServiceProviderId) //Dont know if this will work
                );

            FilterDefinition<Customer> combinedFilter;
            if (serviceProviderIds.Count == 0)
            {
                combinedFilter = organisationFilter;
            }
            else
            {
                combinedFilter = organisationFilter & serviceProviderFilter;
            }

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.Profiles,
                profile => profile.OrganisationId == organisationId
                );

            var result = await this.customerCollection.Find(combinedFilter).Project<CustomerProfile>(project).ToListAsync();

            return result;
        }

        /// <inheritdoc />
        public async Task<ServiceRequest> GetServiceRequest(string appointmentId)
        {

            ServiceRequest serviceRequest = new ServiceRequest();

            var serviceRequestFilter = Builders<Customer>.Filter.ElemMatch(
                cust => cust.ServiceRequests,
                serviceRequest => serviceRequest.AppointmentId == appointmentId
                );

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.ServiceRequests,
                sr => sr.AppointmentId == appointmentId
                );

            var result = await this.customerCollection.Find(serviceRequestFilter).Project<ServiceRequest>(project).FirstOrDefaultAsync();

            return serviceRequest;
        }

        /// <inheritdoc />
        public async Task<List<ServiceRequest>> GetServiceRequestsOfCustomer(string customerId)
        {
            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();

            var custFilter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));

            var project = Builders<Customer>.Projection.Include(_ => true);

            var result = await this.customerCollection
                .Find(custFilter)
                .Project<ServiceRequest>(project)
                .ToListAsync();

            return result;
        }

        /// <inheritdoc />
        public async Task<List<ServiceRequest>> GetServiceRequests(List<string> serviceRequestIds)
        {
            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();

            var objectIdList = new List<ObjectId>();
            foreach (var srId in serviceRequestIds)
            {
                objectIdList.Add(new ObjectId(srId));
            }

            var serviceRequestFilter = Builders<Customer>.Filter.ElemMatch(
                cust => cust.ServiceRequests,
                serviceRequest => objectIdList.Contains(serviceRequest.ServiceRequestId) //Dont know if this will work
                );

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.ServiceRequests,
                serviceRequest => objectIdList.Contains(serviceRequest.ServiceRequestId) //Dont know if this will work
                );

            serviceRequests.AddRange(
                await this.customerCollection
                .Find(serviceRequestFilter)
                .Project<ServiceRequest>(project)
                .ToListAsync()
                );

            return serviceRequests;
        }


        #endregion Customer

        #region Organisation

        /// <inheritdoc />
        public async Task<Organisation> GetOrganisation(string organisationId)
        {
            var orgFilter = Builders<Organisation>.Filter.Eq(org => org.OrganisationId, new ObjectId(organisationId));
            var result = await this.organisationCollection.Find(orgFilter).FirstOrDefaultAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations(string serviceProviderId)
        {
            var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);
            var result = await this.organisationCollection.Find(orgFilter).ToListAsync();
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations()
        {
            var result = await this.organisationCollection.Find(_ => true).ToListAsync();
            return result;
        }

        #endregion Organisation

        #region CrossDocument

        /// <inheritdoc />
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

        #endregion CrossDocument

    }
}
