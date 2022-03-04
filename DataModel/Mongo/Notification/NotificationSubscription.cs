using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataModel.Mongo.Notification
{
    public class NotificationSubscription
    {
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SubscriptionType SubscriptionType { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType EventType { get; set; }
        public List<int> MinuteIntervals { get; set; } //For immediate type, this is null. For 1 hour reminder it is 60, for 30 min its 30, for day its 60*24
        public bool IsEnabledForSelf { get; set; }
        public bool IsEnabledForCustomers { get; set; }

    }
}
