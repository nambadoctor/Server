namespace ClientDataModels.SP
{
    public class OrganisationRole
    {
        public String OrganisationRoleId { get; set; }
        public string Name { get; set; }   
        public string ServiceProviderId { get; set; }
        public string Description { get; set; } 
        public string Permissions { get; set; }
    }
}