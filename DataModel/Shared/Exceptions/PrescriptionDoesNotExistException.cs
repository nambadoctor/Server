using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class PrescriptionDoesNotExistException : Exception
    {
        public PrescriptionDoesNotExistException(string message)
    : base(message)
        {
        }
        public PrescriptionDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
