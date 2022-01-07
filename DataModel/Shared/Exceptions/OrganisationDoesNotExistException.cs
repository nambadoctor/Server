using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class OrganisationDoesNotExistException : Exception
    {
        public OrganisationDoesNotExistException(string message)
    : base(message)
        {
        }
        public OrganisationDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
