using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class CustomerDoesNotExistException : Exception
    {
        public CustomerDoesNotExistException(string message)
    : base(message)
        {
        }
        public CustomerDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
