using DataModel.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Client.Provider
{
    public class CustomerProfile
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateOfBirth DateOfBirth { get; set; }
    }
}
