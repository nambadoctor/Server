using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider.Outgoing
{
    public class OrgansiationBasic
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool IsDefault { get; set; }
    }
}
