using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Report
    {
        [BsonId]
        public ObjectId ReportId { set; get; } //This will be the filename in blob
        public FileInfo FileInfo { get; set; }
        public ReportDetails Details { get; set; }

    }
}
