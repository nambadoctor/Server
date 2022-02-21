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

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType NotificationType { get; set; }
        public string AppointmentId { get; set; }
        public DateTime NotificationScheduledTime { get; set; }
        public DateTime CreatedDateTime { get; set; }

    }
}
