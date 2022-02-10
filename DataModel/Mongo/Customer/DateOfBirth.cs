using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class DateOfBirth
    {
        [BsonId]
        public ObjectId DateOfBirthId { get; set; }

        public DateTime Date { get; set; }
        public string? Age { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
