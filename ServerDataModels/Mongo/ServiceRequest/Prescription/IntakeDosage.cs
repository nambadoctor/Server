using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class IntakeDosage
    {
        [BsonId]
        public ObjectId IntakeDosageId { get; set; }
        public string Dose { get; set; }
        public string Unit { get; set; }
    }
}
