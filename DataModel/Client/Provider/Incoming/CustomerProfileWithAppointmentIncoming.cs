using DataModel.Client.Provider.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class CustomerProfileWithAppointmentIncoming
    {
        public CustomerProfileIncoming CustomerProfileIncoming { get; set; }

        public AppointmentIncoming AppointmentIncoming { get; set; }
    }
}
