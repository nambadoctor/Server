using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Address
    {
        [BsonId]
        public ObjectId AddressId { get; set; }

        public string StreetAddress { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PinCode { get; set; }

        public string Type { get; set; }

        public string GoogleMapsAddress { get; set; }

        public bool IsDeleted { get; set; }

    }
}