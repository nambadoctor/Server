using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.Common;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Report
    {
        [BsonId]
        public ObjectId ReportId { set; get; }
        public FileInfo FileInfo { get; set; }
        public ReportDetails Details { get; set; }

    }
}
