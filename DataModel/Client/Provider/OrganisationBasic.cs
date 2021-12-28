using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider
{
    /// <summary>
    /// This is only for disaply purpose for user to select and switch org
    /// </summary>
    public class OrganisationBasic
    {
        public string ServieProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool IsDefault { get; set; }

    }
}
