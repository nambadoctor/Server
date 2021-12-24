using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Habit
    {
        [BsonId]
        public ObjectId HabitId { get; set; }
        public string AlcoholDetails { get; set; }
        public string SmokingDetails { get; set; }
    }
}
