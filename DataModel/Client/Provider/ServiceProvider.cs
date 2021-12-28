using System.Collections.Generic;
using System.Security.Cryptography;

namespace DataModel.Client.Provider
{
    public class ServiceProvider
    {
       /// <summary>
       /// ID of Service provider in the context
       /// </summary>
        public string ServiceProviderId { get; set; }

        public string OrganisationId { get; set; }

        public List<OrganisationRole> Roles { get; set; }

        /// <summary>
        /// List of Organisations that Service provider is part of. This is to allow switch between organisation 
        /// </summary>

        public ServiceProviderProfile ServiceProviderProfile { get; set; }


    }
}
