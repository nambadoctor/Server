﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Organisation
    {
        [BsonId]
        public ObjectId OrganisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> EmailAddresses { get; set; }
        public string Logo { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Member> Members { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public bool IsDeleted { get; set; }

    }
}
