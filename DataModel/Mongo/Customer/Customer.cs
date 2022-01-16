using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        [BsonId]
        public ObjectId CustomerId { get; set; }
        public List<CustomerProfile> Profiles { get; set; }
        public List<AuthInfo> AuthInfos { get; set; }
        public bool IsDeleted { get; set; }
    }
}
