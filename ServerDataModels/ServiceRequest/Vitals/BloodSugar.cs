using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
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
