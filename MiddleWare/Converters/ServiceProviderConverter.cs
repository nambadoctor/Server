namespace MiddleWare.Converters
{
    public static class ServiceProviderConverter
    {
        public static DataModel.Client.Provider.ServiceProvider ConvertToClientServiceProvider(DataModel.Mongo.ServiceProvider mongoServiceProvider, string organisationId)
        {
            var clientSp = new DataModel.Client.Provider.ServiceProvider();

            clientSp.ServiceProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            clientSp.OrganisationId = organisationId;

            var profile = mongoServiceProvider.Profiles.Find(profile => profile.OrganisationId == organisationId);

            clientSp.FirstName = profile.FirstName;
            clientSp.LastName = profile.LastName;
            clientSp.ProfilePictureUrl = profile.ProfilePictureUrl;
            clientSp.Type = profile.LastName;

            return clientSp;
        }
    }
}
