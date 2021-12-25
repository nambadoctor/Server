using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class TestUser
    {
        [BsonId]
        public ObjectId UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string UserPhoneNumber { get; set; }
        public string TesterType { get; set; }
    }
}
