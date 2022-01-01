using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider
{
    public class ServiceProviderBasic
    {
        public string ServiceProviderId { get; set; }
        public List<OrgansiationBasic> Organsiations { get; set; }
    }
}
