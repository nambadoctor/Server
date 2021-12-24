using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Medicine
    {
        [BsonId]
        public ObjectId MedicineId { get; set; }
        public string Name { get; set; }
        public RouteOfAdministration RouteOfAdministration { get; set; }
        public Duration Duration { get; set; }
        public Intake Intake { get; set; }
        public Notes Notes { get; set; }

    }
}
