﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class ReportDetails
    {
        [BsonId]
        public ObjectId ReportDetailsId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
    }
}
