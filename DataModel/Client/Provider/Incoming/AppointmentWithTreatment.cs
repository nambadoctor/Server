using DataModel.Client.Provider.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Incoming
{
    public class AppointmentWithTreatment
    {
        public TreatmentIncoming TreatmentIncoming { get; set; }

        public AppointmentIncoming AppointmentIncoming { get; set; }
    }
}
