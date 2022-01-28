using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Note
    {
        [BsonId]
        public ObjectId NoteId { set; get; }
        public string NoteText { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
