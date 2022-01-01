using ClientModel = DataModel.Client.Provider;
using ServerModel = DataModel.Mongo;

namespace MiddleWare.Converters
{
    public static class ServiceProviderConverter
    {
        public static ClientModel.ServiceProviderBasic ConvertToClientServiceProviderBasic(ServerModel.ServiceProvider mongoServiceProvider, List<ServerModel.Organisation> organisationList, ServerModel.Organisation defaultOrganisation)
        {
            var serviceProviderBasic = new ClientModel.ServiceProviderBasic();

            serviceProviderBasic.ServiceProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            serviceProviderBasic.Organsiations = new List<ClientModel.OrgansiationBasic>();

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

        public static ClientModel.OrgansiationBasic ConvertOrganisationToOrganisationBasic(ServerModel.Organisation organisation, bool isDefault)
        {
            var organisationBasic = new ClientModel.OrgansiationBasic();

            organisationBasic.Name = organisation.Name;
            organisationBasic.Logo = organisation.Logo;
            organisationBasic.Description = organisation.Description;
            organisationBasic.OrganisationId = organisation.OrganisationId.ToString();
            organisationBasic.IsDefault = isDefault;

            return organisationBasic;
        }
        public static ClientModel.ServiceProvider ConvertToClientServiceProvider(ServerModel.ServiceProviderProfile mongoServiceProvider, ServerModel.Organisation organisation, ServerModel.Member member)
        {
            var clientSp = new ClientModel.ServiceProvider();

            clientSp.ServiceProviderId = mongoServiceProvider.ServiceProviderId;
            clientSp.OrganisationId = organisation.OrganisationId.ToString();

            clientSp.Roles = new List<string>();
            clientSp.Roles.Add(member.Role);

            //Set profile values
            var clientSpProfile = new ClientModel.ServiceProviderProfile();
            clientSpProfile.FirstName = mongoServiceProvider.FirstName;
            clientSpProfile.LastName = mongoServiceProvider.LastName;
            clientSpProfile.ProfilePictureUrl = mongoServiceProvider.ProfilePictureUrl;
            clientSpProfile.Type = mongoServiceProvider.ServiceProviderType;
            clientSpProfile.ServiceProviderId = mongoServiceProvider.ServiceProviderId;
            clientSpProfile.OrganisationId = mongoServiceProvider.OrganisationId;
            clientSpProfile.ServiceProviderProfileId = mongoServiceProvider.ServiceProviderProfileId.ToString();

            clientSp.ServiceProviderProfile = clientSpProfile;

            return clientSp;
        }

        public static ClientModel.ServiceProviderProfile ConvertToClientServiceProviderProfile(ServerModel.ServiceProviderProfile mongoServiceProviderProfile, string serviceProviderId, string organisationId)
        {
            var clientSp = new ClientModel.ServiceProviderProfile();

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
