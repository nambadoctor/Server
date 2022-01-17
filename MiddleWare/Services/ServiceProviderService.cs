using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using Exceptions = DataModel.Shared.Exceptions;
using MongoDB.GenericRepository.Interfaces;

namespace MiddleWare.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private IServiceProviderRepository serviceProviderRepository;
        private IOrganisationRepository organisationRepository;
        private ILogger logger;

        public ServiceProviderService(IServiceProviderRepository serviceProviderRepository, IOrganisationRepository organisationRepository, ILogger<ServiceProviderService> logger)
        {
            this.serviceProviderRepository = serviceProviderRepository;
            this.organisationRepository = organisationRepository;
            this.logger = logger;
        }
        public async Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemberships()
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderService:GetServiceProviderOrganisationMemeberships"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                var serviceProvider = await serviceProviderRepository.GetServiceProviderFromPhoneNumber(NambaDoctorContext.PhoneNumber);

                DataValidation.ValidateObject(serviceProvider);

                logger.LogInformation("Found service provider id {0}", serviceProvider.ServiceProviderId);
                NambaDoctorContext.AddTraceContext("ServiceProviderId", serviceProvider.ServiceProviderId.ToString());

                var organisationList = await organisationRepository.GetOrganisationsOfServiceProvider(serviceProvider.ServiceProviderId.ToString());

                if (organisationList == null || organisationList.Count <= 0)
                {
                    logger.LogError("No organisation found for service providerId: {0}",
                        serviceProvider.ServiceProviderId);

                    throw new Exceptions.ResourceNotFoundException
                        (string.Format("Service provider {0} is not part of any organisations", serviceProvider.ServiceProviderId));
                }

                var defaultOrganisation = organisationList.First();

                NambaDoctorContext.AddTraceContext("DefaultOrganisationId", defaultOrganisation.OrganisationId.ToString());
                NambaDoctorContext.AddTraceContext("DefaultOrganisationName", defaultOrganisation.Name);

                logger.LogInformation("Set default organisation Name: {0} Id : {1}", defaultOrganisation.Name, defaultOrganisation.OrganisationId);

                //Buid client Object

                var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProviderBasic(
                    serviceProvider,
                    organisationList,
                    defaultOrganisation
                    );

                logger.LogInformation("converted to ConvertToClientServiceProviderBasic");
                return clientServiceProvider;

            }

        }
        public async Task<ProviderClientOutgoing.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderService:GetServiceProviderAsync"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceProviderId, IdType.ServiceProvider);
                DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

                NambaDoctorContext.AddTraceContext("OrganisationId", OrganisationId);
                NambaDoctorContext.AddTraceContext("ServiceProviderId", ServiceProviderId);

                var serviceProviderProfile = await serviceProviderRepository.GetServiceProviderProfile(ServiceProviderId, OrganisationId);

                DataValidation.ValidateObject(serviceProviderProfile);

                var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                    serviceProviderProfile
                    );

                return clientServiceProvider;

            }

        }
    }
}
