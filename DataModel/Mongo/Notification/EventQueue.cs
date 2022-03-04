﻿using MongoDB.Bson;
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
        public DateTime CreatedDateTime { get; set; }

    }
}