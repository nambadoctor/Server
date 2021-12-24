using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.Organisation
{
    [BsonIgnoreExtraElements]
    public class Member
    {
        [BsonId]
        public ObjectId MemberId { get; set; }
        public string ServiceProviderId { get; set; }
        public string Role { get; set; }
    }
}
