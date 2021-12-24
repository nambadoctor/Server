using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.Other
{
    [BsonIgnoreExtraElements]
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
    }
}
