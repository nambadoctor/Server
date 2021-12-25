using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
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
