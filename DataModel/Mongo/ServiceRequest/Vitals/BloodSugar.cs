using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class BloodSugar
    {
        [BsonId]
        public ObjectId BloodSugarId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
