using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class ReportDetails
    {
        [BsonId]
        public ObjectId ReportDetailsId { get; set; }
        public DateTime? UploadedDateTime { get; set; }
    }
}
