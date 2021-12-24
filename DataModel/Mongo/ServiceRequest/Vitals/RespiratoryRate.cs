using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class RespiratoryRate
    {
        [BsonId]
        public ObjectId RespiratoryRateId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
