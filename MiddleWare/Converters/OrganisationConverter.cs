using ClientModel = DataModel.Client.Provider;
using ServerModel = DataModel.Mongo;
namespace MiddleWare.Converters
{
    public static class OrganisationConverter
    {
        public static ClientModel.Organisation ConvertToClientOrganisation(ServerModel.Organisation mongoOrganisation, List<DataModel.Mongo.ServiceProvider> serviceProviders)
        {
            var clientOrganisation = new ClientModel.Organisation();

            clientOrganisation.OrganisationId = mongoOrganisation.OrganisationId.ToString();
            clientOrganisation.Name = mongoOrganisation.Name;
            clientOrganisation.Description = mongoOrganisation.Description;
            clientOrganisation.Logo = mongoOrganisation.Logo;

            var serviceProvidersInOrg = new List<ClientModel.ServiceProviderProfile>();

            foreach (var serviceProvider in serviceProviders)
            {
                //Find the profile for the org inside ServiceProvider object
                var spProfile = (from serviceProviderProfile in serviceProvider.Profiles
                                 where serviceProviderProfile.OrganisationId == mongoOrganisation.OrganisationId.ToString()
                                 select serviceProviderProfile).SingleOrDefault();

                serviceProvidersInOrg.Add(
                        ServiceProviderConverter.ConvertToClientServiceProviderProfile(
                            spProfile,
                            serviceProvider.ServiceProviderId.ToString(),
                            mongoOrganisation.OrganisationId.ToString())
                    );
            }

            clientOrganisation.Profiles = serviceProvidersInOrg;

            return clientOrganisation;
        }

        public static List<ClientModel.Organisation> ConvertToClientOrganisationList(List<ServerModel.Organisation> mongoOrganisations, List<ServerModel.ServiceProvider> serviceProviders)
        {
            var clientOrgList = new List<ClientModel.Organisation>();

            foreach (var mongoOrganisation in mongoOrganisations)
            {
                clientOrgList.Add(ConvertToClientOrganisation(mongoOrganisation, serviceProviders));
            }

            return clientOrgList;
        }
    }
}
