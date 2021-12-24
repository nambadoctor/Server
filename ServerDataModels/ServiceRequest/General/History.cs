using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class History
    {
        [BsonId]
        public ObjectId MedicalHistoryId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
