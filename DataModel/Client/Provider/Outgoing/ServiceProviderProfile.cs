using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider.Outgoing
{
    public class ServiceProviderProfile
    {
        public string OrganisationId { get; set; }
        public string ServiceProviderId { get; set; }
        public string ServiceProviderProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
