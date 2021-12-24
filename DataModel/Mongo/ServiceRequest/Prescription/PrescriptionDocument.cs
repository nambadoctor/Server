using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.Common;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class PrescriptionDocument
    {
        [BsonId]
        public ObjectId PrescriptionDocumentId { get; set; }
        public FileInfo FileInfo { get; set; }
        public PrescriptionDetail PrescriptionDetail { get; set; }

    }
}
