﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class CustomerProfile
    {
        [BsonId]
        public ObjectId CustomerProfileId { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateOfBirth DateOfBirth { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string? OrganisationId { get; set; }
    }
}
