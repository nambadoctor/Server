using DataModel.Mongo;
using DataModel.Shared;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BaseMongoDBDataLayer : IMongoDbDataLayer
    {

        public IMongoDatabase dbInstance;

        public IMongoCollection<ServiceProvider> serviceProviderCollection;
        public IMongoCollection<Customer> customerCollection;
        public IMongoCollection<Organisation> organisationCollection;


        public NambaDoctorContext _nambaDoctorContext;

        private MongoClient mongoClient;

        private ILogger logger;

        public IMongoDatabase DbInstance
        {
            get
            {
                return dbInstance;
            }
        }

        public BaseMongoDBDataLayer(ILogger<BaseMongoDBDataLayer> logger)
        {
            var mongoDBInstance = new MongoDBInstance();

            dbInstance = mongoDBInstance.GetMongoDB();
            mongoClient = mongoDBInstance.GetMongoClient();

            this.serviceProviderCollection = dbInstance.GetCollection<ServiceProvider>(ConnectionConfiguration.ServiceProvideCollection);
            this.customerCollection = dbInstance.GetCollection<Customer>(ConnectionConfiguration.CustomerCollection);
            this.organisationCollection = dbInstance.GetCollection<Organisation>(ConnectionConfiguration.OrganisationCollection);

            this.logger = logger;
        }

        #region ServiceProvider

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProvider(string serviceProviderId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProvider"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");
                    var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));
                    var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<ServiceProvider>> GetServiceProviders(List<ObjectId> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderAvailabilities"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");
                    var filter = Builders<ServiceProvider>.Filter.In(sp => sp.ServiceProviderId, serviceProviderIds);
                    var result = await this.serviceProviderCollection.Find(filter).ToListAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderFromRegisteredPhoneNumber"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var spFilter = Builders<ServiceProvider>.Filter.ElemMatch(sp => sp.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);

                    var result = await this.serviceProviderCollection.Find(spFilter).FirstOrDefaultAsync();

                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());

                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<ServiceProviderProfile> GetServiceProviderProfile(string serviceProviderId, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    ServiceProviderProfile serviceProviderProfile = null;

                    var spFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

                    var project = Builders<ServiceProvider>.Projection.ElemMatch(
                        sp => sp.Profiles,
                        profile => profile.OrganisationId == organisationId
                        );

                    var serviceProvider = await this.serviceProviderCollection.Find(spFilter).Project<ServiceProvider>(project).FirstOrDefaultAsync();

                    if (serviceProvider != null && serviceProvider.Profiles != null)
                    {
                        serviceProviderProfile = serviceProvider.Profiles.FirstOrDefault();
                    }

                    return serviceProviderProfile;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());

                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(List<string> serviceProviderIds, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderProfiles"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");
                    var serviceProviderIdList = new List<ObjectId>();

                    foreach (var serviceProviderId in serviceProviderIds)
                    {
                        serviceProviderIdList.Add(new ObjectId(serviceProviderId));
                    }

                    var organisationAppointmentFilter = Builders<ServiceProvider>.Filter.ElemMatch(
                       sp => sp.Profiles,
                       profile => profile.OrganisationId == organisationId
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

                    var project = Builders<ServiceProvider>.Projection.Expression(
                        sp => sp.Profiles.Where(profile => profile.OrganisationId == organisationId)
                        );

                    var result = await this.serviceProviderCollection.Aggregate().Match(combinedFilter).Project(project).ToListAsync();

                    var listOfProfiles = new List<ServiceProviderProfile>();

                    foreach (var profiles in result)
                    {
                        listOfProfiles.AddRange(profiles);
                    }

                    return listOfProfiles;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<ServiceProviderAvailability>> GetServiceProviderAvailabilities(string serviceProviderId, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderAvailabilities"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");
                    var filter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

                    var project = Builders<ServiceProvider>.Projection.Expression(
                        sp => sp.Availabilities.Where(availability => availability.OrganisationId == organisationId)
                        );

                    var result = await this.serviceProviderCollection.Aggregate().Match(filter).Project(project).ToListAsync();

                    var availabilities = new List<ServiceProviderAvailability>();

                    foreach (var availability in result)
                    {
                        availabilities.AddRange(availability);
                    }

                    return availabilities;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var serviceProviderFilter = Builders<ServiceProvider>.Filter.Eq(sp => sp.ServiceProviderId, new ObjectId(serviceProviderId));

                    var project = Builders<ServiceProvider>.Projection.ElemMatch(
                        sp => sp.Appointments,
                        appointment => appointment.AppointmentId == new ObjectId(appointmentId)
                        );

                    var serviceProvider = await this.serviceProviderCollection.Find(serviceProviderFilter).Project<ServiceProvider>(project).FirstOrDefaultAsync();

                    if (serviceProvider != null && serviceProvider.Appointments != null)
                        return serviceProvider.Appointments.FirstOrDefault();
                    else
                        return null;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<Appointment>> GetAppointmentsForServiceProvider(string organisationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetAppointmentsForServiceProvider"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

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

                    var project = Builders<ServiceProvider>.Projection.Expression(
                        sp => sp.Appointments.Where(appointment => appointment.OrganisationId == organisationId)
                        );

                    var result = await this.serviceProviderCollection.Aggregate().Match(combinedFilter).Project(project).ToListAsync();

                    var appointments = new List<Appointment>();

                    foreach (var sppointments in result)
                    {
                        appointments.AddRange(sppointments);
                    }

                    return appointments;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        public async Task<Appointment> SetAppointment(Appointment appointment)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:SetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    if (appointment.AppointmentId == ObjectId.Empty)
                    {
                        appointment.AppointmentId = ObjectId.GenerateNewId();
                    }

                    var filter = Builders<ServiceProvider>.Filter;

                    var nestedFilter = Builders<ServiceProvider>.Filter.And(
                        filter.Eq(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId)),
                        filter.ElemMatch(sp => sp.Appointments, a => a.AppointmentId.Equals(appointment.AppointmentId)));

                    var update = Builders<ServiceProvider>.Update.Set(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));

                    if (appointment.ServiceRequestId != null)
                    {
                        update = update.Set("Appointments.$.ServiceRequestId", appointment.ServiceRequestId);
                    }

                    if (appointment.CustomerId != null)
                    {
                        update = update.Set("Appointments.$.CustomerId", appointment.CustomerId);
                    }

                    if (appointment.OrganisationId != null)
                    {
                        update = update.Set("Appointments.$.OrganisationId", appointment.OrganisationId);
                    }

                    if (appointment.Status != null)
                    {
                        update = update.Set("Appointments.$.Status", appointment.Status);
                    }

                    if (appointment.AppointmentType != null)
                    {
                        update = update.Set("Appointments.$.AppointmentType", appointment.AppointmentType);
                    }

                    if (appointment.PaymentDetail != null)
                    {
                        update = update.Set("Appointments.$.PaymentDetail", appointment.PaymentDetail);
                    }

                    if (appointment.AddressId != null)
                    {
                        update = update.Set("Appointments.$.AddressId", appointment.AddressId);
                    }

                    if (appointment.ScheduledAppointmentStartTime != null)
                    {
                        update = update.Set("Appointments.$.ScheduledAppointmentStartTime", appointment.ScheduledAppointmentStartTime);
                    }

                    if (appointment.ScheduledAppointmentEndTime != null)
                    {
                        update = update.Set("Appointments.$.ScheduledAppointmentEndTime", appointment.ScheduledAppointmentEndTime);
                    }

                    if (appointment.ActualAppointmentStartTime != null)
                    {
                        update = update.Set("Appointments.$.ActualAppointmentStartTime", appointment.ActualAppointmentStartTime);
                    }

                    if (appointment.ActualAppointmentEndTime != null)
                    {
                        update = update.Set("Appointments.$.ActualAppointmentEndTime", appointment.ActualAppointmentEndTime);
                    }

                    if (appointment.Cancellation != null)
                    {
                        appointment.Cancellation.CancellationID = ObjectId.GenerateNewId();
                        update = update.Set("Appointments.$.Cancellation", appointment.Cancellation);
                    }



                    var result = await this.serviceProviderCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

                    return appointment;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        #endregion ServiceProvider

        #region Customer

        /// <inheritdoc />
        public async Task<Customer> GetCustomerFromRegisteredPhoneNumber(string phoneNumber)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomerFromRegisteredPhoneNumber"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var custFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.AuthInfos, authInfo => authInfo.AuthId == phoneNumber);
                    var result = await this.customerCollection.Find(custFilter).FirstOrDefaultAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<Customer> GetCustomer(string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var custFilter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));
                    var result = await this.customerCollection.Find(custFilter).FirstOrDefaultAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<Customer>> GetCustomers(List<string> customerIds)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomers"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var objectIdList = new List<ObjectId>();
                    foreach (var custId in customerIds)
                    {
                        objectIdList.Add(new ObjectId(custId));
                    }
                    var filter = Builders<Customer>.Filter.In(cust => cust.CustomerId, objectIdList);
                    var result = await this.customerCollection.Find(filter).ToListAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<List<Customer>> GetCustomersAddedByOrganisation(string organisationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomersAddedByOrganisation"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var organisationFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => profile.OrganisationId == organisationId);

                    var spFilter = Builders<CustomerProfile>.Filter.In(cust => cust.ServiceProviderId, serviceProviderIds);

                    var serviceProviderFilter = Builders<Customer>.Filter.ElemMatch(
                        cust => cust.Profiles,
                        spFilter
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

                    var customers = await this.customerCollection.Find(combinedFilter).ToListAsync();

                    return customers;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var filter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));

                    var project = Builders<Customer>.Projection.ElemMatch(
                        cust => cust.Profiles,
                        profile => profile.OrganisationId.Equals(organisationId)
                        );

                    var customer = await this.customerCollection.Find(filter).Project<Customer>(project).FirstOrDefaultAsync();

                    if (customer != null && customer.Profiles != null)
                        return customer.Profiles.FirstOrDefault();
                    else
                        return null;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<CustomerProfile>> GetCustomerProfiles(List<string> customerIds, string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomerProfiles"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var customerIdList = new List<ObjectId>();

                    foreach (var customerId in customerIds)
                    {
                        customerIdList.Add(new ObjectId(customerId));
                    }

                    var filter = Builders<Customer>.Filter.In(cust => cust.CustomerId, customerIdList);

                    var project = Builders<Customer>.Projection.Expression(
                        cust => cust.Profiles.Where(profile => profile.OrganisationId == organisationId)
                        );

                    var result = await this.customerCollection.Aggregate().Match(filter).Project(project).ToListAsync();

                    var profiles = new List<CustomerProfile>();

                    foreach (var profileList in result)
                    {
                        profiles.AddRange(profileList);
                    }

                    return profiles;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<CustomerProfile>> GetCustomerProfilesAddedByOrganisation(string organisationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetCustomerProfilesAddedByOrganisation"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");
                    var organisationFilter = Builders<Customer>.Filter.ElemMatch(cust => cust.Profiles, profile => profile.OrganisationId == organisationId);

                    var spFilter = Builders<CustomerProfile>.Filter.In(custProfile => custProfile.ServiceProviderId, serviceProviderIds);

                    var serviceProviderFilter = Builders<Customer>.Filter.ElemMatch(
                        cust => cust.Profiles,
                        spFilter
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

                    var project = Builders<Customer>.Projection.Expression(
                        cust => cust.Profiles.Where(profile => profile.OrganisationId == organisationId)
                        );

                    var result = await this.customerCollection.Aggregate().Match(combinedFilter).Project(project).ToListAsync();

                    var profiles = new List<CustomerProfile>();

                    foreach (var profileList in result)
                    {
                        profiles.AddRange(profileList);
                    }

                    return profiles;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<CustomerProfile> SetCustomerProfile(CustomerProfile customerProfile)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:SetCustomerProfile"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    ObjectId customerId;
                    if (string.IsNullOrWhiteSpace(customerProfile.CustomerId))
                    {
                        customerId = ObjectId.GenerateNewId();
                        customerProfile.CustomerId = customerId.ToString();
                        await SetCustomerWithAuthInfo(customerId, customerProfile.PhoneNumbers.FirstOrDefault());
                    }
                    else
                    {
                        customerId = new ObjectId(customerProfile.CustomerId);
                    }

                    if (customerProfile.CustomerProfileId == ObjectId.Empty)
                    {
                        customerProfile.CustomerProfileId = ObjectId.GenerateNewId();
                        var filter = Builders<Customer>.Filter;
                        var nestedFilter = filter.And(
                            filter.Eq(sp => sp.CustomerId, customerId));
                        var update = Builders<Customer>.Update.AddToSet(cust => cust.Profiles, customerProfile);
                        var result = await this.customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });
                        return customerProfile;
                    }
                    else
                    {
                        var filter = Builders<Customer>.Filter;

                        var nestedFilter = filter.And(
                            filter.Eq(cust => cust.CustomerId, customerId),
                            filter.ElemMatch(cust => cust.Profiles, profile => profile.CustomerProfileId == customerProfile.CustomerProfileId));

                        var update = Builders<Customer>.Update.Set(cust => cust.CustomerId, customerId);

                        if (customerProfile.CustomerId != null)
                        {
                            update = update.Set("Profiles.$.CustomerId", customerProfile.CustomerId);
                        }

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

                        if (customerProfile.PhoneNumbers != null)
                        {
                            update = update.Set("Profiles.$.PhoneNumbers", customerProfile.PhoneNumbers);
                        }

                        if (customerProfile.Addresses != null)
                        {
                            update = update.Set("Profiles.$.Addresses", customerProfile.Addresses);
                        }

                        if (customerProfile.EmailAddress != null)
                        {
                            update = update.Set("Profiles.$.EmailAddress", customerProfile.EmailAddress);
                        }

                        if (customerProfile.OrganisationId != null)
                        {
                            update = update.Set("Profiles.$.OrganisationId", customerProfile.OrganisationId);
                        }

                        if (customerProfile.ServiceProviderId != null)
                        {
                            update = update.Set("Profiles.$.ServiceProviderId", customerProfile.ServiceProviderId);
                        }

                        var result = await this.customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

                        return customerProfile;
                    }

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        private async Task SetCustomerWithAuthInfo(ObjectId customerId, PhoneNumber phoneNumber)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:SetCustomerWithAuthInfo"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var customer = new Customer();

                    customer.CustomerId = customerId;
                    customer.AuthInfos = new List<AuthInfo>();

                    var authInfo = new AuthInfo();
                    authInfo.AuthInfoId = ObjectId.GenerateNewId();
                    authInfo.AuthId = phoneNumber.CountryCode + phoneNumber.Number;
                    authInfo.AuthType = "PhoneNumber";

                    customer.AuthInfos.Add(authInfo);
                    customer.NotificationInfos = new List<NotificationInfo>();
                    customer.Profiles = new List<CustomerProfile>();
                    customer.ServiceRequests = new List<ServiceRequest>();

                    await this.customerCollection.InsertOneAsync(customer);

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<ServiceRequest> GetServiceRequest(string appointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceRequest"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    ServiceRequest serviceRequest = new ServiceRequest();

                    var serviceRequestFilter = Builders<Customer>.Filter.ElemMatch(
                        cust => cust.ServiceRequests,
                        serviceRequest => serviceRequest.AppointmentId == appointmentId
                        );

                    var project = Builders<Customer>.Projection.ElemMatch(
                        cust => cust.ServiceRequests,
                        sr => sr.AppointmentId == appointmentId
                        );

                    var customer = await this.customerCollection.Find(serviceRequestFilter).Project<Customer>(project).FirstOrDefaultAsync();

                    if (customer != null && customer.ServiceRequests != null)
                        return customer.ServiceRequests.FirstOrDefault();
                    else
                        return null;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<List<ServiceRequest>> GetServiceRequestsOfCustomer(string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceRequestsOfCustomer"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    List<ServiceRequest> serviceRequests = new List<ServiceRequest>();

                    var custFilter = Builders<Customer>.Filter.Eq(cust => cust.CustomerId, new ObjectId(customerId));

                    var project = Builders<Customer>.Projection.Include(_ => true);

                    var customer = await this.customerCollection
                        .Find(custFilter)
                        .Project<Customer>(project)
                        .FirstOrDefaultAsync();

                    return customer.ServiceRequests;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<ServiceRequest> SetServiceRequest(ServiceRequest serviceRequest)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetServiceProviderAvailabilities"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    ObjectId customerId;
                    if (string.IsNullOrWhiteSpace(serviceRequest.CustomerId))
                    {
                        customerId = ObjectId.GenerateNewId();
                        serviceRequest.CustomerId = customerId.ToString();
                    }
                    else
                    {
                        customerId = new ObjectId(serviceRequest.CustomerId);
                    }

                    if (serviceRequest.ServiceRequestId == ObjectId.Empty)
                    {
                        serviceRequest.ServiceRequestId = ObjectId.GenerateNewId();
                        var filter = Builders<Customer>.Filter;
                        var nestedFilter = filter.And(
                            filter.Eq(sp => sp.CustomerId, customerId));
                        var update = Builders<Customer>.Update.AddToSet(cust => cust.ServiceRequests, serviceRequest);
                        var result = await this.customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });
                        return serviceRequest;

                    }
                    else
                    {
                        var filter = Builders<Customer>.Filter;

                        var nestedFilter = filter.And(
                            filter.Eq(cust => cust.CustomerId, customerId),
                            filter.ElemMatch(cust => cust.ServiceRequests, sr => sr.ServiceRequestId == serviceRequest.ServiceRequestId));

                        var update = Builders<Customer>.Update.Set(cust => cust.CustomerId, customerId);

                        if (serviceRequest.CustomerId != null)
                        {
                            update = update.Set("ServiceRequests.$.CustomerId", serviceRequest.CustomerId);
                        }

                        if (serviceRequest.ServiceProviderId != null)
                        {
                            update = update.Set("ServiceRequests.$.ServiceProviderId", serviceRequest.ServiceProviderId);
                        }

                        if (serviceRequest.AppointmentId != null)
                        {
                            update = update.Set("ServiceRequests.$.AppointmentId", serviceRequest.AppointmentId);
                        }

                        if (serviceRequest.Reason != null)
                        {
                            update = update.Set("ServiceRequests.$.Reason", serviceRequest.Reason);
                        }

                        if (serviceRequest.Examination != null)
                        {
                            update = update.Set("ServiceRequests.$.Examination", serviceRequest.Examination);
                        }

                        if (serviceRequest.Allergies != null)
                        {
                            update = update.Set("ServiceRequests.$.Allergies", serviceRequest.Allergies);
                        }

                        if (serviceRequest.Histories != null)
                        {
                            update = update.Set("ServiceRequests.$.Histories", serviceRequest.Histories);
                        }

                        if (serviceRequest.Diagnosis != null)
                        {
                            update = update.Set("ServiceRequests.$.Diagnosis", serviceRequest.Diagnosis);
                        }

                        if (serviceRequest.Vitals != null)
                        {
                            update = update.Set("ServiceRequests.$.Vitals", serviceRequest.Vitals);
                        }

                        if (serviceRequest.AdditionalDetails != null)
                        {
                            update = update.Set("ServiceRequests.$.AdditionalDetails", serviceRequest.AdditionalDetails);
                        }

                        if (serviceRequest.Advices != null)
                        {
                            update = update.Set("ServiceRequests.$.Advices", serviceRequest.Advices);
                        }

                        if (serviceRequest.MedicineList != null)
                        {
                            update = update.Set("ServiceRequests.$.MedicineList", serviceRequest.MedicineList);
                        }

                        if (serviceRequest.Reports != null)
                        {
                            update = update.Set("ServiceRequests.$.Reports", serviceRequest.Reports);
                        }

                        if (serviceRequest.PrescriptionDocuments != null)
                        {
                            update = update.Set("ServiceRequests.$.PrescriptionDocuments", serviceRequest.PrescriptionDocuments);
                        }

                        var result = await this.customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

                        return serviceRequest;
                    }

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }


        #endregion Customer

        #region Organisation

        /// <inheritdoc />
        public async Task<Organisation> GetOrganisation(string organisationId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetOrganisation"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var orgFilter = Builders<Organisation>.Filter.Eq(org => org.OrganisationId, new ObjectId(organisationId));
                    var result = await this.organisationCollection.Find(orgFilter).FirstOrDefaultAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations(string serviceProviderId)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetOrganisations"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var orgFilter = Builders<Organisation>.Filter.ElemMatch(org => org.Members, member => member.ServiceProviderId == serviceProviderId);
                    var result = await this.organisationCollection.Find(orgFilter).ToListAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<List<Organisation>> GetOrganisations()
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetOrganisations"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var result = await this.organisationCollection.Find(_ => true).ToListAsync();
                    return result;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        #endregion Organisation

        #region CrossDocument

        /// <inheritdoc />
        public async Task<string> GetUserTypeFromRegisteredPhoneNumber(string phoneNumber)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:GetUserTypeFromRegisteredPhoneNumber"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

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
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        /// <inheritdoc />
        public async Task<Appointment> SetAppointmentWithServiceRequest(Appointment appointment, ServiceRequest serviceRequest)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:SetAppointmentWithServiceRequest"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    await AddAppointmentWithSession(null, appointment);
                    await AddServiceRequestWithSession(null, serviceRequest);

                    return appointment;

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        public async Task<(CustomerProfile, Appointment)> SetCustomerWithAppointment(CustomerProfile customerProfile, Appointment appointment, ServiceRequest serviceRequest)
        {
            using (logger.BeginScope("Method: {Method}", "BaseMongoDBDataLayer:SetCustomerWithAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    logger.LogInformation("DB Execution start");

                    var customerId = await AddCustomerProfileWithSession(null, customerProfile);
                    appointment.CustomerId = customerId;
                    serviceRequest.CustomerId = customerId;

                    await AddAppointmentWithSession(null, appointment);

                    await AddServiceRequestWithSession(null, serviceRequest);

                    return (customerProfile, appointment);

                }
                catch (Exception ex)
                {
                    logger.LogInformation("DB execution end with Exception: {0}", ex.ToString());
                    throw;
                }
                finally
                {
                    logger.LogInformation("DB Execution end");
                }
            }

        }

        #endregion CrossDocument

        #region Private methods
        private async Task<bool> AddServiceRequestWithSession(IClientSessionHandle session, ServiceRequest serviceRequest)
        {

            var filter = Builders<Customer>.Filter;

            var nestedFilter = filter.Eq(cust => cust.CustomerId, new ObjectId(serviceRequest.CustomerId));

            var update = Builders<Customer>.Update.AddToSet(cust => cust.ServiceRequests, serviceRequest);

            var srResult = await customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

            return srResult.IsAcknowledged;
        }

        private async Task<bool> AddAppointmentWithSession(IClientSessionHandle session, Appointment appointment)
        {
            var filter = Builders<ServiceProvider>.Filter;

            var nestedFilter = filter.Eq(sp => sp.ServiceProviderId, new ObjectId(appointment.ServiceProviderId));

            var update = Builders<ServiceProvider>.Update.AddToSet(sp => sp.Appointments, appointment);

            var result = await serviceProviderCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

            return result.IsAcknowledged;
        }

        private async Task<string> AddCustomerProfileWithSession(IClientSessionHandle session, CustomerProfile customerProfile)
        {
            ObjectId customerId;
            if (string.IsNullOrWhiteSpace(customerProfile.CustomerId))
            {
                customerId = ObjectId.GenerateNewId();
                customerProfile.CustomerId = customerId.ToString();
                await SetCustomerWithAuthInfo(customerId, customerProfile.PhoneNumbers.FirstOrDefault());
            }
            else
            {
                customerId = new ObjectId(customerProfile.CustomerId);
            }

            if (customerProfile.CustomerProfileId == ObjectId.Empty)
            {
                customerProfile.CustomerProfileId = ObjectId.GenerateNewId();
                var filter = Builders<Customer>.Filter;
                var nestedFilter = filter.And(
                    filter.Eq(sp => sp.CustomerId, customerId));
                var update = Builders<Customer>.Update.AddToSet(cust => cust.Profiles, customerProfile);
                var result = await this.customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });
            }
            else
            {
                var filter = Builders<Customer>.Filter;

                var nestedFilter = filter.And(
                    filter.Eq(cust => cust.CustomerId, customerId),
                    filter.ElemMatch(cust => cust.Profiles, profile => profile.CustomerProfileId == customerProfile.CustomerProfileId));

                var update = Builders<Customer>.Update.Set(cust => cust.CustomerId, customerId);

                if (customerProfile.CustomerId != null)
                {
                    update = update.Set("Profiles.$.CustomerId", customerProfile.CustomerId);
                }

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

                if (customerProfile.PhoneNumbers != null)
                {
                    update = update.Set("Profiles.$.PhoneNumbers", customerProfile.PhoneNumbers);
                }

                if (customerProfile.Addresses != null)
                {
                    update = update.Set("Profiles.$.Addresses", customerProfile.Addresses);
                }

                if (customerProfile.EmailAddress != null)
                {
                    update = update.Set("Profiles.$.EmailAddress", customerProfile.EmailAddress);
                }

                if (customerProfile.OrganisationId != null)
                {
                    update = update.Set("Profiles.$.OrganisationId", customerProfile.OrganisationId);
                }

                if (customerProfile.ServiceProviderId != null)
                {
                    update = update.Set("Profiles.$.ServiceProviderId", customerProfile.ServiceProviderId);
                }

                var result = await customerCollection.UpdateOneAsync(nestedFilter, update, new UpdateOptions { IsUpsert = true });

            }

            return customerId.ToString();
        }
        #endregion Private methods

    }
}
