using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class AppointmentDoesNotExistException : Exception
    {
        public AppointmentDoesNotExistException(string message)
    : base(message)
        {
        }
        public AppointmentDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
