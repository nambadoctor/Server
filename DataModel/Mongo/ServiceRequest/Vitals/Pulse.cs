using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Pulse
    {
        [BsonId]
        public ObjectId PulseId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
