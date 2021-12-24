using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class AdditionalDetail
    {
        [BsonId]
        public ObjectId AdditionalDetailId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
