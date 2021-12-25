namespace MiddleWare.Converters
{
    public static class OrganisationConverter
    {
        public static DataModel.Client.Provider.Organisation ConvertToClientOrganisation(DataModel.Mongo.Organisation mongoOrganisation)
        {
            var clientOrganisation = new DataModel.Client.Provider.Organisation();

            clientOrganisation.OrganisationId = mongoOrganisation.OrganisationId.ToString();
            clientOrganisation.Name = mongoOrganisation.Name;
            clientOrganisation.Description = mongoOrganisation.Description;
            clientOrganisation.Logo = mongoOrganisation.Logo;

            var roles = new List<DataModel.Client.Provider.OrganisationRole>();

            foreach (var member in mongoOrganisation.Members)
            {
                var role = new DataModel.Client.Provider.OrganisationRole();
                role.ServiceProviderId = member.ServiceProviderId;
                role.Name = member.Role;
                role.Description = member.Role;//Make a description here
                role.Permissions = ""; //Define permissions

                roles.Add(role);
            }

            clientOrganisation.Roles = roles;

            return clientOrganisation;
        }

        public static List<DataModel.Client.Provider.Organisation> ConvertToClientOrganisationList(List<DataModel.Mongo.Organisation> mongoOrganisations)
        {
            var clientOrgList = new List<DataModel.Client.Provider.Organisation>();

            foreach (var mongoOrganisation in mongoOrganisations)
            {
                clientOrgList.Add(ConvertToClientOrganisation(mongoOrganisation));
            }

            return clientOrgList;
        }
    }
}
