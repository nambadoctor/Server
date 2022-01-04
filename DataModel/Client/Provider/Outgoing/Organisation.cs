using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider.Outgoing
{
    public class Organisation
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }

        /// <summary>
        /// This will be list of all service providers based on caller permission
        /// </summary>
        public List<ServiceProviderProfile> Profiles { get; set; }
    }
}
