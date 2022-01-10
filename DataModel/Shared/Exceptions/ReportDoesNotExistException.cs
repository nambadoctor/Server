using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class ReportDoesNotExistException : Exception
    {
        public ReportDoesNotExistException(string message)
    : base(message)
        {
        }
        public ReportDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
