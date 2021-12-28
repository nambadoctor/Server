namespace MiddleWare.Converters
{
    public static class ServiceProviderConverter
    {
        public static DataModel.Client.Provider.ServiceProvider ConvertToClientServiceProvider(DataModel.Mongo.ServiceProvider mongoServiceProvider, DataModel.Mongo.Organisation organisation)
        {
            var clientSp = new DataModel.Client.Provider.ServiceProvider();

            clientSp.ServiceProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            clientSp.OrganisationId = organisation.OrganisationId.ToString();

            //Find role in org
            var role = organisation.Members.Find(member => member.ServiceProviderId == mongoServiceProvider.ServiceProviderId.ToString());
            //TODO Construct role for client here based on some logic

            //Find profile to map
            var mongoProfile = mongoServiceProvider.Profiles.Find(profile => profile.OrganisationId == organisation.OrganisationId.ToString());
            if (mongoProfile == null)
            {
                throw new Exception("No profile found with mathcing organisation id in ConvertToClientServiceProvider");
            }

            //Set profile values
            var clientSpProfile = new DataModel.Client.Provider.ServiceProviderProfile();
            clientSpProfile.FirstName = mongoProfile.FirstName;
            clientSpProfile.LastName = mongoProfile.LastName;
            clientSpProfile.ProfilePictureUrl = mongoProfile.ProfilePictureUrl;
            clientSpProfile.Type = mongoProfile.ServiceProviderType;

            clientSp.ServiceProviderProfile = clientSpProfile;

            return clientSp;
        }

        public static DataModel.Client.Provider.ServiceProviderProfile ConvertToClientServiceProviderProfile(DataModel.Mongo.ServiceProviderProfile mongoServiceProviderProfile, string serviceProviderId, string organisationId)
        {
            var clientSp = new DataModel.Client.Provider.ServiceProviderProfile();

            clientSp.ServiceProviderId = serviceProviderId;
            clientSp.OrganisationId = organisationId;

            clientSp.FirstName = mongoServiceProviderProfile.FirstName;
            clientSp.LastName = mongoServiceProviderProfile.LastName;
            clientSp.ProfilePictureUrl = mongoServiceProviderProfile.ProfilePictureUrl;
            clientSp.Type = mongoServiceProviderProfile.ServiceProviderType;

            return clientSp;
        }
    }
}
