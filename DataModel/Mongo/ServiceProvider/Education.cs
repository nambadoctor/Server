using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceProvider
{
    [BsonIgnoreExtraElements]
    public class Education
    {
        [BsonId]
        public ObjectId EducationId { get; set; }

        public string Course { get; set; }

        public int? Year { get; set; }

        public string Country { get; set; }

        public string College { get; set; }

        public string University { get; set; }

        public bool IsDeleted { get; set; }

    }
}
