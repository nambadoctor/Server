using DataModel.Mongo;
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
        /// Get service providers from list of IDs passed
        /// </summary>
        /// <param name="serviceProviderIds"></param>
        /// <returns></returns>
        public Task<List<ServiceProvider>> GetServiceProviders(List<string> serviceProviderIds);

        /// <summary>
        /// Get service provider document from phone number in format +[CCode][10-digit number] with no spaces
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<ServiceProvider> GetServiceProviderFromRegisteredPhoneNumber(string phoneNumber);

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
        /// Get service request matching Id
        /// </summary>
        /// <param name="serviceRequestId"></param>
        /// <returns></returns>
        public Task<ServiceRequest> GetServiceRequest(string serviceRequestId);

        /// <summary>
        /// Get list of all service requests owned by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<List<ServiceRequest>> GetServiceRequestsOfCustomer(string customerId);

        /// <summary>
        /// Get all service requests matching Ids
        /// </summary>
        /// <param name="serviceRequestIds"></param>
        /// <returns></returns>
        public Task<List<ServiceRequest>> GetServiceRequests(List<string> serviceRequestIds);

        /// <summary>
        /// Get single appointment data from Id
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        public Task<(ServiceProvider, Appointment, Customer, ServiceRequest)> GetAppointmentData(string serviceProviderId, string appointmentId);

        /// <summary>
        /// Get list of appointment data for organisation and fitler with list of service providers. 
        /// Send empty list for removing service provider filter.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="serviceProviderIds"></param>
        /// <returns></returns>
        public Task<List<(ServiceProvider, Appointment, Customer, ServiceRequest)>> GetAppointmentsForServiceProvider(string organisationId, List<string> serviceProviderIds);
    }
}
