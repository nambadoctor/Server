using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class FileInfo
    {
        [BsonId]
        public ObjectId? FileInfoId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    }
}
