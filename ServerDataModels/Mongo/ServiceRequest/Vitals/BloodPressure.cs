using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class BloodPressure
    {
        [BsonId]
        public ObjectId BloodPressureId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
