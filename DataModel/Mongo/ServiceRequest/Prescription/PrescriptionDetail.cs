using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class PrescriptionDetail
    {
        [BsonId]
        public ObjectId PrescriptionDetailId { get; set; }
        public DateTime? UploadedDateTime { get; set; }
    }
}
