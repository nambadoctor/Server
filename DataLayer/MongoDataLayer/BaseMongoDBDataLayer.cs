using MongoDB.Bson;
using MongoDB.Driver;
using DataModel.Mongo;

using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Shared;
using System.Linq;

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

        #region ServiceProvider

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProvider(string serviceProviderId)
        {
            _NDLogger.LogEvent("Start GetServiceProvider");
            var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            _NDLogger.LogEvent("End GetServiceProvider");
            return result;
        }

        /// <inheritdoc />
        public async Task<List<ServiceProvider>> GetServiceProviders(List<string> serviceProviderIds)
        {
            _NDLogger.LogEvent("Start GetServiceProviders with matching Ids");
            var objectIdList = new List<ObjectId>();
            foreach (var spId in serviceProviderIds)
            {
                objectIdList.Add(new ObjectId(spId));
            }
            var filter = Builders<ServiceProvider>.Filter.In(sp => sp.ServiceProviderId, objectIdList);
            var result = await this.serviceProviderCollection.Find(filter).ToListAsync();
            _NDLogger.LogEvent("End GetServiceProviders with matching Ids");
            return result;
        }

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber)
        {
            var spFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
            var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
            return result;
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

            var serviceProviderFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => serviceProviderIds.Contains(profile.ServiceProviderId));

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
        public async Task<ServiceRequest> GetServiceRequest(string serviceRequestId)
        {

            ServiceRequest serviceRequest = new ServiceRequest();

            var serviceRequestFilter = Builders<Customer>.Filter.ElemMatch(
                cust => cust.ServiceRequests,
                serviceRequest => serviceRequest.ServiceRequestId == new ObjectId(serviceRequestId)
                );

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.ServiceRequests,
                sr => sr.ServiceRequestId == new ObjectId(serviceRequestId)
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
                serviceRequest => objectIdList.Contains(serviceRequest.ServiceRequestId)
                );

            var project = Builders<Customer>.Projection.ElemMatch(
                cust => cust.ServiceRequests,
                serviceRequest => objectIdList.Contains(serviceRequest.ServiceRequestId)
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
            _NDLogger.LogEvent($"Start GetOrganisation : {organisationId}");
            var orgFilter = Builders<Organisation>.Filter.Eq(org => org.OrganisationId, new ObjectId(organisationId));
            var result = await this.organisationCollection.Find(orgFilter).FirstOrDefaultAsync();
            _NDLogger.LogEvent($"End GetOrganisation : {organisationId}");
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations(string serviceProviderId)
        {
            _NDLogger.LogEvent("Start GetOrganisations");
            var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);
            var result = await this.organisationCollection.Find(orgFilter).ToListAsync();
            _NDLogger.LogEvent($"End GetOrganisations. Returned {result.Count} organisations");
            return result;
        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations()
        {
            _NDLogger.LogEvent("Start GetOrganisations");
            var result = await this.organisationCollection.Find(_ => true).ToListAsync();
            _NDLogger.LogEvent($"End GetOrganisations. Returned {result.Count} organisations");
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

        //Dont use. Testing

        /// <inheritdoc />
        public async Task<(ServiceProvider, Appointment, Customer, ServiceRequest)> GetAppointmentData(string serviceProviderId, string appointmentId)
        {
            var serviceProviderFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
            var serviceProvider = await this.serviceProviderCollection.Find(serviceProviderFilter).FirstOrDefaultAsync();

            var appointment = (from app in serviceProvider.Appointments
                               where app.AppointmentId == new ObjectId(appointmentId)
                               select app).FirstOrDefault();

            var customerFilter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(appointment.CustomerId));
            var customer = await this.customerCollection.Find(customerFilter).FirstOrDefaultAsync();

            var serviceRequest = (from sr in customer.ServiceRequests
                                  where sr.AppointmentId == appointmentId
                                  select sr).FirstOrDefault();

            return (serviceProvider, appointment, customer, serviceRequest);
        }

        /// <inheritdoc />
        public async Task<List<(ServiceProvider, Appointment, Customer, ServiceRequest)>> GetAppointmentsForServiceProvider(string organisationId, List<string> serviceProviderIds)
        {
            var objectIdList = new List<ObjectId>();
            foreach (var spId in serviceProviderIds)
            {
                objectIdList.Add(new ObjectId(spId));
            }

            //Filter according to org and service provider
            var appointmentDataList = new List<(ServiceProvider, Appointment, Customer, ServiceRequest)>();

            var organisationAppointmentFilter = Builders<ServiceProvider>.Filter.ElemMatch(
                sp => sp.Appointments,
                appointment => appointment.OrganisationId == organisationId
                );

            var serviceProviderFilter = Builders<ServiceProvider>.Filter.In(
                sp => sp.ServiceProviderId,
                objectIdList
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

            var serviceProviders = await this.serviceProviderCollection.Find(combinedFilter).ToListAsync();

            //Get customer ids to fetch from appointment list
            var customerIds = new List<string>();

            foreach (var serviceProvider in serviceProviders)
            {
                if (serviceProvider.Appointments != null)
                    foreach (var appointment in serviceProvider.Appointments)
                    {
                        customerIds.Add(appointment.CustomerId);
                    }
            }

            var customers = await GetCustomers(customerIds);

            foreach (var serviceProvider in serviceProviders)
            {
                if (serviceProvider.Appointments != null)
                    foreach (var appointment in serviceProvider.Appointments)
                    {
                        var customer = (from cust in customers
                                        where cust.CustomerId == new ObjectId(appointment.CustomerId)
                                        select cust).FirstOrDefault();

                        var serviceRequest = (from sr in customer.ServiceRequests
                                              where sr.AppointmentId == appointment.AppointmentId.ToString()
                                              select sr).FirstOrDefault();

                        appointmentDataList.Add((serviceProvider, appointment, customer, serviceRequest));
                    }
            }

            return appointmentDataList;
        }

        #endregion CrossDocument

    }
}
