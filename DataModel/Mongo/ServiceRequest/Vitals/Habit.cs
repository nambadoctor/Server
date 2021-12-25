using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
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
