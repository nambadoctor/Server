using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class PhoneNumber
    {
        [BsonId]
        public ObjectId PhoneNumberId { get; set; }

        public string CountryCode { get; set; }

        public string Number { get; set; }

        public string Type { get; set; }

        public bool IsDeleted { get; set; }
    }
}
