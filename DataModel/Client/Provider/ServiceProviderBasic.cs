using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider
{
    public class ServiceProviderBasic
    {
        public string ServieProviderId { get; set; }
        public List<OrgansiationBasic> Organsiations { get; set; }
    }
}
