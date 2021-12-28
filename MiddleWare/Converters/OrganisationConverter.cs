namespace MiddleWare.Converters
{
    public static class OrganisationConverter
    {
        public static DataModel.Client.Provider.Organisation ConvertToClientOrganisation(DataModel.Mongo.Organisation mongoOrganisation, List<DataModel.Mongo.ServiceProvider> serviceProviders)
        {
            var clientOrganisation = new DataModel.Client.Provider.Organisation();

            clientOrganisation.OrganisationId = mongoOrganisation.OrganisationId.ToString();
            clientOrganisation.Name = mongoOrganisation.Name;
            clientOrganisation.Description = mongoOrganisation.Description;
            clientOrganisation.Logo = mongoOrganisation.Logo;

            var serviceProvidersInOrg = new List<DataModel.Client.Provider.ServiceProviderProfile>();

            foreach (var serviceProvider in serviceProviders)
            {
                var spProfile = (from serviceProviderProfile in serviceProvider.Profiles
                                 where serviceProviderProfile.OrganisationId == mongoOrganisation.OrganisationId.ToString()
                                 select serviceProviderProfile).SingleOrDefault();

                if (spProfile == null)
                {
                    continue;
                }

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

        public static List<DataModel.Client.Provider.Organisation> ConvertToClientOrganisationList(List<DataModel.Mongo.Organisation> mongoOrganisations, List<DataModel.Mongo.ServiceProvider> serviceProviders)
        {
            var clientOrgList = new List<DataModel.Client.Provider.Organisation>();

            foreach (var mongoOrganisation in mongoOrganisations)
            {
                clientOrgList.Add(ConvertToClientOrganisation(mongoOrganisation, serviceProviders));
            }

            return clientOrgList;
        }
    }
}
