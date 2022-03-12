using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace DataModel.Mongo.Notification
{
    [BsonIgnoreExtraElements]
    public class NotificationQueue
    {
        [BsonId]
        public ObjectId NotificationQueueId { get; set; }
        public string? AppointmentId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType NotificationType { get; set; }
        public string UserPhoneNumber { get; set; }
        public string Message { get; set; } // Can be name of self or person interacting with appointment
        public string SenderId { get; set; } //NmbaDr OR NMBADR
        public DateTime ToBeNotifiedTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
