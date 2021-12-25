using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Height
    {
        [BsonId]
        public ObjectId HeightId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
