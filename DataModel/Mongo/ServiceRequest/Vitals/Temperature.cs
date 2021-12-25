using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
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
