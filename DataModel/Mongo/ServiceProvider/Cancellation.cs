using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Cancellation
    {
        [BsonId]
        public ObjectId CancellationID { get; set; }
        public string ReasonName { get; set; }
        public string CancelledBy { get; set; }
        public string CancelledByType { get; set; }
        public string Notes { get; set; }
    }
}
