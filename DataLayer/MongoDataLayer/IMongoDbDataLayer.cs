using DataModel.Mongo;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IMongoDbDataLayer
    {
        /// <summary>
        /// Get service provider document by id
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        public Task<ServiceProvider> GetServiceProvider(string serviceProviderId);

        /// <summary>
        /// Get service provider profile by id for organisation
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        public Task<ServiceProviderProfile> GetServiceProviderProfile(string serviceProviderId, string organisationId);

        /// <summary>
        /// Get service providers from list of IDs passed
        /// </summary>
        /// <param name="serviceProviderIds"></param>
        /// <returns></returns>
        public Task<List<ServiceProvider>> GetServiceProviders(List<ObjectId> serviceProviderIds);

        /// <summary>
        /// Get service provider profiles from list of IDs passed
        /// Organisation id is mandatory
        /// </summary>
        /// <param name="serviceProviderIds"></param>
        /// <returns></returns>
        public Task<List<ServiceProviderProfile>> GetServiceProviderProfiles(List<string> serviceProviderIds, string organisationId);

        /// <summary>
        /// Get service provider document from phone number in format +[CCode][10-digit number] with no spaces
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get availability for service provider and filter by org id
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public Task<List<ServiceProviderAvailability>> GetServiceProviderAvailabilities(string serviceProviderId, string organisationId);

        /// <summary>
        /// Get organsiation document from Id
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public Task<Organisation> GetOrganisation(string organisationId);

        /// <summary>
        /// Get list of organisations service provider is part of
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        public Task<List<Organisation>> GetOrganisations(string serviceProviderId);

        /// <summary>
        /// Get all organisations without a filter
        /// </summary>
        /// <returns></returns>
        public Task<List<Organisation>> GetOrganisations();

        /// <summary>
        /// Cross query customer and service provider document by Phone number in format +[CCode][10-digit number] with no spaces
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<string> GetUserTypeFromRegisteredPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get customer document from phone number in format +[CCode][10-digit number] with no spaces
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<Customer> GetCustomerFromRegisteredPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get customer document by id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<Customer> GetCustomer(string customerId);

        /// <summary>
        /// Get customer profile by id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<CustomerProfile> GetCustomerProfile(string customerId, string organisationId);

        /// <summary>
        /// Get list of customer profiles matching list of customer ids and organisation id
        /// </summary>
        /// <param name="customerIds"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public Task<List<CustomerProfile>> GetCustomerProfiles(List<string> customerIds, string organisationId);

        /// <summary>
        /// Get customers from list of IDs passed
        /// </summary>
        /// <param name="customerIds"></param>
        /// <returns></returns>
        public Task<List<Customer>> GetCustomers(List<string> customerIds);

        /// <summary>
        /// Here serviceprovider list can be empty if only filter by org is needed.
        /// Org id is mandatory.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        public Task<List<Customer>> GetCustomersAddedByOrganisation(string organisationId, List<string> serviceProviderIds);

        /// <summary>
        /// Here serviceprovider list can be empty if only filter by org is needed.
        /// Org id is mandatory.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        public Task<List<CustomerProfile>> GetCustomerProfilesAddedByOrganisation(string organisationId, List<string> serviceProviderIds);

        /// <summary>
        /// Set customer profile
        /// </summary>
        /// <param name="customerProfile"></param>
        /// <returns></returns>
        public Task<CustomerProfile> SetCustomerProfile(CustomerProfile customerProfile);

        /// <summary>
        /// Get service request matching appointment Id
        /// </summary>
        /// <param name="serviceRequestId"></param>
        /// <returns></returns>
        public Task<ServiceRequest> GetServiceRequest(string appointmentId);

        /// <summary>
        /// Set service request for customer
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public Task<ServiceRequest> SetServiceRequest(ServiceRequest serviceRequest);

        /// <summary>
        /// Get list of all service requests owned by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<List<ServiceRequest>> GetServiceRequestsOfCustomer(string customerId);

        /// <summary>
        /// Get single appointment data from Id
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        public Task<Appointment> GetAppointment(string serviceProviderId, string appointmentId);

        /// <summary>
        /// Get list of appointment data for organisation and fitler with list of service providers. 
        /// Send empty list for removing service provider filter.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="serviceProviderIds"></param>
        /// <returns></returns>
        public Task<List<Appointment>> GetAppointmentsForServiceProvider(string organisationId, List<string> serviceProviderIds);

        /// <summary>
        /// Set appointment
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        public Task<Appointment> SetAppointment(Appointment appointment);

        /// <summary>
        /// Set appointment and service request. 
        /// Only to be used when setting it first time as it requires modification across multiple collection.
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public Task<Appointment> SetAppointmentWithServiceRequest(Appointment appointment, ServiceRequest serviceRequest);

        /// <summary>
        /// Sets an appointment and service request along with customer as a transaction
        /// </summary>
        /// <param name="customerProfile"></param>
        /// <param name="appointment"></param>
        /// <returns></returns>
        public Task<(CustomerProfile, Appointment)> SetCustomerWithAppointment(CustomerProfile customerProfile, Appointment appointment, ServiceRequest serviceRequest);

    }
}
