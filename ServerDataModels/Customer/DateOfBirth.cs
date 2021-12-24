using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.Customer
{
    [BsonIgnoreExtraElements]
    public class DateOfBirth
    {
        [BsonId]
        public ObjectId DateOfBirthId { get; set; }

        public int? Day { get; set; } //1-31

        public int? Month { get; set; } //1-12

        public int? Year { get; set; } //YYYY

    }
}
