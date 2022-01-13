﻿using DataLayer;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using Client = DataModel.Client.Provider;

namespace MiddleWare.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private IMongoDbDataLayer datalayer;
        private ILogger logger;

        public ServiceProviderService(IMongoDbDataLayer dataLayer, ILogger<ServiceProviderService> logger)
        {
            this.datalayer = dataLayer;
            this.logger = logger;
        }
        public async Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemeberships()
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderService:GetServiceProviderOrganisationMemeberships"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    var serviceProvider = await datalayer.GetServiceProviderFromRegisteredPhoneNumber(NambaDoctorContext.PhoneNumber);

                    if (serviceProvider == null)
                    {
                        logger.LogError("Service provider does not exist for the phonumber: {0}",
                            NambaDoctorContext.PhoneNumber);

                        throw new ServiceProviderDoesnotExistsException
                            (string.Format("Service provider does not exist for phone number {0}", NambaDoctorContext.PhoneNumber));

                    }

                    logger.LogInformation("Found service provider id {0}", serviceProvider.ServiceProviderId);
                    NambaDoctorContext.AddTraceContext("ServiceProviderId", serviceProvider.ServiceProviderId.ToString());

                    var organisationList = await datalayer.GetOrganisations(serviceProvider.ServiceProviderId.ToString());

                    if (organisationList == null)
                    {
                        logger.LogError("No organisation found for service providerId: {0}",
                            serviceProvider.ServiceProviderId);

                        throw new ServiceProviderOrgsDoesnotExistsException
                            (string.Format("Service provider {0} is not part of any organisations", serviceProvider.ServiceProviderId));
                    }

                    var defaultOrganisation = organisationList.FirstOrDefault();


                    if (defaultOrganisation == null)
                    {
                        throw new ServiceProviderOrgsDoesnotExistsException
                            (string.Format("Service provider {0} does not have default organisation", serviceProvider.ServiceProviderId));

                    }

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
                finally
                {

                }
            }

        }
        public async Task<ProviderClientOutgoing.ServiceProvider> GetServiceProviderAsync(string ServiceProviderId, string OrganisationId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderService:GetServiceProviderAsync"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ServiceProviderId) || !ObjectId.TryParse(ServiceProviderId, out var spid))
                    {
                        throw new ArgumentNullException("ServiceProviderId is null or empty or not well formed objectId");
                    }

                    if (string.IsNullOrWhiteSpace(OrganisationId) || !ObjectId.TryParse(OrganisationId, out var orgid))
                    {
                        throw new ArgumentNullException("OrganisationId is null or empty or not well formed objectId");
                    }

                    NambaDoctorContext.AddTraceContext("OrganisationId", OrganisationId);
                    NambaDoctorContext.AddTraceContext("ServiceProviderId", ServiceProviderId);
                    var serviceProviderProfile = await datalayer.GetServiceProviderProfile(ServiceProviderId, OrganisationId);

                    if (serviceProviderProfile == null)
                    {
                        throw new ServiceProviderDoesnotExistsException($"Serviceprovider not found with id: {ServiceProviderId}");
                    }

                    var organisation = await datalayer.GetOrganisation(OrganisationId);

                    if (organisation == null)
                    {
                        throw new ServiceProviderOrgsDoesnotExistsException($"Organisation not found with id: {OrganisationId}");
                    }

                    //Find role in org
                    var role = organisation.Members.Find(member => member.ServiceProviderId == ServiceProviderId);
                    if (role == null)
                    {
                        throw new KeyNotFoundException($"No role found for this service provider({ServiceProviderId}) in organisation with id: {OrganisationId}");
                    }

                    //Buid client Object
                    var clientServiceProvider = ServiceProviderConverter.ConvertToClientServiceProvider(
                        serviceProviderProfile,
                        organisation,
                        role
                        );

                    return clientServiceProvider;
                }
                finally
                {

                }
            }

        }
    }
}
