using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class BlobStorageException : Exception
    {
        public BlobStorageException(string message)
    : base(message)
        {
        }
        public BlobStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
