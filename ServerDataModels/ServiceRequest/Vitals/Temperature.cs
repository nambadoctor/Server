using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Temperature
    {
        [BsonId]
        public ObjectId TemperatureId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
