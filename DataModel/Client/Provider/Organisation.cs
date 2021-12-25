
using System.Collections.Generic;

namespace DataModel.Client.Provider
{
    public class Organisation
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }

        public List<OrganisationRole> Roles { get; set; }

    }
}
