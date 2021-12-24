using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Duration
    {
        [BsonId]
        public ObjectId DurationId { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
