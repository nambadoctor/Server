using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Weight
    {
        [BsonId]
        public ObjectId WeightId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
