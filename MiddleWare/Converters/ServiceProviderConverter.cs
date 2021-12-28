namespace MiddleWare.Converters
{
    public static class ServiceProviderConverter
    {
        public static DataModel.Client.Provider.ServiceProviderBasic ConvertToClientServiceProviderBasic(DataModel.Mongo.ServiceProvider mongoServiceProvider, List<DataModel.Mongo.Organisation> organisationList, DataModel.Mongo.Organisation defaultOrganisation)
        {
            var serviceProviderBasic = new DataModel.Client.Provider.ServiceProviderBasic();

            serviceProviderBasic.ServieProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            serviceProviderBasic.Organsiations = new List<DataModel.Client.Provider.OrgansiationBasic>();

            foreach (var organisation in organisationList)
            {
                serviceProviderBasic.Organsiations.Add(
                    ConvertOrganisationToOrganisationBasic(
                        organisation,
                        organisation.OrganisationId == defaultOrganisation.OrganisationId
                        )
                    );
            }

            return serviceProviderBasic;
        }

        public static DataModel.Client.Provider.OrgansiationBasic ConvertOrganisationToOrganisationBasic(DataModel.Mongo.Organisation organisation, bool isDefault)
        {
            var organisationBasic = new DataModel.Client.Provider.OrgansiationBasic();

            organisationBasic.Name = organisation.Name;
            organisationBasic.Logo = organisation.Logo;
            organisationBasic.Description = organisation.Description;
            organisationBasic.OrganisationId = organisation.OrganisationId.ToString();
            organisationBasic.IsDefault = isDefault;

            return organisationBasic;
        }
        public static DataModel.Client.Provider.ServiceProvider ConvertToClientServiceProvider(DataModel.Mongo.ServiceProvider mongoServiceProvider, DataModel.Mongo.Organisation organisation)
        {
            var clientSp = new DataModel.Client.Provider.ServiceProvider();

            clientSp.ServiceProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            clientSp.OrganisationId = organisation.OrganisationId.ToString();

            //Find role in org
            var role = organisation.Members.Find(member => member.ServiceProviderId == mongoServiceProvider.ServiceProviderId.ToString());
            if (role == null)
            {
                throw new Exception("No role found for this service provider in organisation");
            }
            clientSp.Roles.Add(role.Role);

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
