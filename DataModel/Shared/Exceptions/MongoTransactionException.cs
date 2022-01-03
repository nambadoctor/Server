using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared.Exceptions
{
    public class MongoTransactionException : Exception
    {
        public MongoTransactionException(string message)
    : base(message)
        {
        }
        public MongoTransactionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
