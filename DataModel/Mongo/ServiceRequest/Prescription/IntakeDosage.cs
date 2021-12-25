using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
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
