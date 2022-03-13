using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ServerModel = DataModel.Mongo;

namespace MiddleWare.Converters
{
    public static class ServiceProviderConverter
    {
        public static ProviderClientOutgoing.ServiceProviderBasic ConvertToClientServiceProviderBasic(ServerModel.ServiceProvider mongoServiceProvider, List<ServerModel.Organisation> organisationList, ServerModel.Organisation defaultOrganisation)
        {
            var serviceProviderBasic = new ProviderClientOutgoing.ServiceProviderBasic();

            serviceProviderBasic.ServiceProviderId = mongoServiceProvider.ServiceProviderId.ToString();
            serviceProviderBasic.Organisations = new List<ProviderClientOutgoing.OrgansiationBasic>();

            foreach (var organisation in organisationList)
            {
                serviceProviderBasic.Organisations.Add(
                    ConvertOrganisationToOrganisationBasic(
                        organisation,
                        organisation.OrganisationId == defaultOrganisation.OrganisationId
                        )
                    );
            }

            return serviceProviderBasic;
        }

        public static ProviderClientOutgoing.OrgansiationBasic ConvertOrganisationToOrganisationBasic(ServerModel.Organisation organisation, bool isDefault)
        {
            var organisationBasic = new ProviderClientOutgoing.OrgansiationBasic();

            organisationBasic.Name = organisation.Name;
            organisationBasic.Logo = organisation.Logo;
            organisationBasic.Description = organisation.Description;
            organisationBasic.OrganisationId = organisation.OrganisationId.ToString();
            organisationBasic.IsDefault = isDefault;

            return organisationBasic;
        }
        public static ProviderClientOutgoing.ServiceProvider ConvertToClientServiceProvider(ServerModel.ServiceProviderProfile mongoServiceProvider)
        {
            var clientSp = new ProviderClientOutgoing.ServiceProvider();

            clientSp.ServiceProviderId = mongoServiceProvider.ServiceProviderId;

            //Set profile values
            var clientSpProfile = new ProviderClientOutgoing.ServiceProviderProfile();
            clientSpProfile.FirstName = mongoServiceProvider.FirstName;
            clientSpProfile.LastName = mongoServiceProvider.LastName;
            clientSpProfile.ProfilePictureUrl = mongoServiceProvider.ProfilePictureUrl;
            clientSpProfile.Type = mongoServiceProvider.ServiceProviderType;
            clientSpProfile.ServiceProviderId = mongoServiceProvider.ServiceProviderId;
            clientSpProfile.OrganisationId = mongoServiceProvider.OrganisationId;
            clientSpProfile.ServiceProviderProfileId = mongoServiceProvider.ServiceProviderProfileId.ToString();
            clientSpProfile.Roles = mongoServiceProvider.Roles;

            clientSp.ServiceProviderProfile = clientSpProfile;

            return clientSp;
        }

        public static ProviderClientOutgoing.ServiceProviderProfile ConvertToClientServiceProviderProfile(ServerModel.ServiceProviderProfile mongoServiceProviderProfile, string serviceProviderId, string organisationId)
        {
            var clientSp = new ProviderClientOutgoing.ServiceProviderProfile();

            clientSp.ServiceProviderId = serviceProviderId;
            clientSp.OrganisationId = organisationId;

            clientSp.FirstName = mongoServiceProviderProfile.FirstName;
            clientSp.LastName = mongoServiceProviderProfile.LastName;
            clientSp.ProfilePictureUrl = mongoServiceProviderProfile.ProfilePictureUrl;
            clientSp.Type = mongoServiceProviderProfile.ServiceProviderType;

            clientSp.Roles = mongoServiceProviderProfile.Roles;

            return clientSp;
        }

        public static List<ProviderClientOutgoing.ServiceProviderProfile> ConvertToClientServiceProviderProfiles(
            List<(ServerModel.ServiceProviderProfile, string, string)> mongoServiceProviderProfiles)
        {
            var serviceProviders = new List<ProviderClientOutgoing.ServiceProviderProfile>();

            foreach (var serviceProvider in mongoServiceProviderProfiles)
            {
                serviceProviders.Add(ConvertToClientServiceProviderProfile(serviceProvider.Item1, serviceProvider.Item2, serviceProvider.Item3));
            }

            return serviceProviders;
        }
    }
}
