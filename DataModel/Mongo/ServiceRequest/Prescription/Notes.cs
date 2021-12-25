using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    public class Notes
    {
        [BsonId]
        public ObjectId NotesId { get; set; }
        public string Value { get; set; }
        public string Details { get; set; }
    }
}
