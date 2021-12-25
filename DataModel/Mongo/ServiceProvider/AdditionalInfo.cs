using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class AdditionalInfo
    {
        [BsonId]
        public ObjectId AdditionalInfoId { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Symptoms { get; set; }
        public List<string> Designations { get; set; }
        public List<string> Specialties { get; set; }
        public List<string> Certifications { get; set; }
        public List<string> ClubMemberships { get; set; }
        public List<string> Procedures { get; set; }
        public List<string> Published { get; set; }
        public List<string> Links { get; set; }
        public string Description { get; set; }
    }
}
