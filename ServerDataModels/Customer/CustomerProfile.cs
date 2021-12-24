using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.Common;
using System.Collections.Generic;

namespace ServerDataModels.Customer
{
    [BsonIgnoreExtraElements]
    public class CustomerProfile
    {
        [BsonId]
        public ObjectId CustomerProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOfBirth DateOfBirth { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<Address> Addresses { get; set; }
        public List<string> Languages { get; set; }
        public string EmailAddress { get; set; }
        public string ProfilePicURL { get; set; }
        public string? OrganisationId { get; set; }
        public string? ServiceProviderId { get; set; }
    }
}
