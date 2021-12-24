﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Height
    {
        [BsonId]
        public ObjectId HeightId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
