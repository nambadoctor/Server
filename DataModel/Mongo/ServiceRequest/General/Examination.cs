using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Examination
    {
        [BsonId]
        public ObjectId ExaminationId { get; set; }
        public string ExaminationDetails { get; set; }
        public string ExaminationType { get; set; }
    }
}
