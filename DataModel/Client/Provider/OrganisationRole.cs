
namespace DataModel.Client.Provider
{
    public class OrganisationRole
    {
        public string OrganisationRoleId { get; set; }
        public string Name { get; set; }   
        public string ServiceProviderId { get; set; }
        public string Description { get; set; } 
        public string Permissions { get; set; }
    }
}