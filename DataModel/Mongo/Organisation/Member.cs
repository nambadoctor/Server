using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Member
    {
        [BsonId]
        public ObjectId MemberId { get; set; }
        public string ServiceProviderId { get; set; }
        public List<string> Roles { get; set; }
    }
}
