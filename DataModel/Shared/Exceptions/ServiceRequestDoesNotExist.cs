using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class ServiceRequestDoesNotExist : Exception
    {
        public ServiceRequestDoesNotExist(string message)
    : base(message)
        {
        }
        public ServiceRequestDoesNotExist(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
