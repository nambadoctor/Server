using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class PhoneNumberBelongsToServiceProviderException : Exception
    {
        public PhoneNumberBelongsToServiceProviderException(string message)
    : base(message)
        {
        }
        public PhoneNumberBelongsToServiceProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
