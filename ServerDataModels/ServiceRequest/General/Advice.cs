using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Advice
    {
        [BsonId]
        public ObjectId AdviceId { get; set; }
        public string Value { get; set; }
        public string Details { get; set; }
    }
}
