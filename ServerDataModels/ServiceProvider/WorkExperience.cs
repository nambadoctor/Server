using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ServerDataModels.ServiceProvider
{
    [BsonIgnoreExtraElements]
    public class WorkExperience
    {
        [BsonId]
        public ObjectId WorkExperienceId { get; set; }

        public string Organization { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
