using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Saturation
    {
        [BsonId]
        public ObjectId SaturationId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
