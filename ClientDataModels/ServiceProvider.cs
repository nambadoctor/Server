using ServerDataModels.Organisation;
using ServerDataModels.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDataModels
{
    public class ServiceProvider
    {
        public string ServiceProviderId { get; set; }
        public List<ServiceProviderProfile> Profiles { get; set; }
        public List<Organisation> Organisations { get; set; }
    }
}
