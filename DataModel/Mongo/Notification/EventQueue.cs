using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace DataModel.Mongo.Notification
{
    [BsonIgnoreExtraElements]
    public class EventQueue
    {

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType EventType { get; set; }
        public string AppointmentId { get; set; }
        public string CustomerId { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string RecieverNumber { get; set; }
        public string CustomMessage { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ScheduledDateTime { get; set; }
    }
}
