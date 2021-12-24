using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.Other
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
