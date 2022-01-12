using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider.Outgoing
{
    public class ServiceProviderBasic
    {
        public string ServiceProviderId { get; set; }
        public List<OrgansiationBasic> Organisations { get; set; }
    }
}
