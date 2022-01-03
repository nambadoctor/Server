using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider
{
    public class PhoneNumber
    {
        public string PhoneNumberId { get; set; }

        public string CountryCode { get; set; }

        public string Number { get; set; }

        public string Type { get; set; }
    }
}
