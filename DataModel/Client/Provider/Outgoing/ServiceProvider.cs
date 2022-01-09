﻿using System.Collections.Generic;
using System.Security.Cryptography;

namespace DataModel.Client.Provider.Outgoing
{
    public class ServiceProvider
    {
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public List<string> Roles { get; set; }
        public ServiceProviderProfile ServiceProviderProfile { get; set; }

    }
}