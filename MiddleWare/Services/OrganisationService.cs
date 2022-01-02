using DataLayer;
using DataModel.Client.Provider;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;

namespace MiddleWare.Services
{
    public class OrganisationService : IOrganisationService
    {
        private IMongoDbDataLayer datalayer;
        private ILogger logger;

        public OrganisationService(IMongoDbDataLayer dataLayer, ILogger<OrganisationService> logger)
        {
            this.datalayer = dataLayer;
            this.logger = logger;
        }

        public async Task<Organisation> GetOrganisationAsync(string OrganisationId)
        {
            using (logger.BeginScope("Method: {Method}", "ServiceProviderService:GetServiceProviderOrganisationMemeberships"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(OrganisationId) || !ObjectId.TryParse(OrganisationId, out var spid))
                    {
                        logger.LogError("OrganisationID is null or empty of not well formed");
                        throw new ArgumentException("Organisation Id was null or not well formed");
                    }
                    NambaDoctorContext.AddTraceContext("OrganisationId", OrganisationId);


                    var organisation = await datalayer.GetOrganisation(OrganisationId);

                    if (organisation == null)
                    {
                        logger.LogError("Organisation not found for Id {0}" , OrganisationId);
                        throw new InvalidDataException($"Organisation not found for id: {OrganisationId}");
                    }


                    var listOfServiceProviderIds = ProcessDBCollection.GetMemberIds(organisation);

                    logger.LogInformation("Found {0} members in Organisation :{1}", listOfServiceProviderIds.Count.ToString(), OrganisationId);

                    var serviceProviders = await datalayer.GetServiceProviders(listOfServiceProviderIds);

                    if(serviceProviders == null)
                    {
                        logger.LogError("Missing service providers for given ServiceProviderIDs");
                        throw new InvalidDataException("Missing service providers for given ServiceProviderIDs");
                    }
                    else
                    {
                        if(serviceProviders.Count != listOfServiceProviderIds.Count)
                        {
                            logger.LogError("Mismatch in number of service providers returned. Requsted {0}, Returened {1}",
                                                                            listOfServiceProviderIds.ToString(), serviceProviders.Count.ToString());

                            throw new InvalidDataException(String.Format("Mismatch in number of service providers returned. Requsted {0}, Returened {1}", 
                                                                            listOfServiceProviderIds.ToString(), serviceProviders.Count.ToString()));
                        }
                    }

                    var clientOrganisation = OrganisationConverter.ConvertToClientOrganisation(
                        organisation, serviceProviders);

                    return clientOrganisation;


                }
                catch (Exception ex)
                {
                    logger.LogError("Exception: {0}" , ex.ToString());

                    throw;
                }
                finally
                {

                }
            }



        }
    }
}
