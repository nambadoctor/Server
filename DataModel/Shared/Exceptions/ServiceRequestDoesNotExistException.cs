using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class ServiceRequestDoesNotExistException : Exception
    {
        public ServiceRequestDoesNotExistException(string message)
    : base(message)
        {
        }
        public ServiceRequestDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
