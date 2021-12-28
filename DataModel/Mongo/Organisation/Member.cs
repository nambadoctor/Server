using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Member
    {
        [BsonId]
        public ObjectId MemberId { get; set; }
        public string ServiceProviderId { get; set; }
        public string Role { get; set; }//Admin, Sp, Secretary
    }
}
