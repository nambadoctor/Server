using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class PrescriptionDetail
    {
        [BsonId]
        public ObjectId PrescriptionDetailId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
    }
}
