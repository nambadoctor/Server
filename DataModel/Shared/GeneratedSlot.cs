using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System;
using System.Text.Json.Serialization;

namespace DataModel.Shared
{
    public class GeneratedSlot
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }
        public string AddressId { get; set; }
        public string OrganisationId { get; set; }
        public double? ServiceFees { get; set; }

    }
}
