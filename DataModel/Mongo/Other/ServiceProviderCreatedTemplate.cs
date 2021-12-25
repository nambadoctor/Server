using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using  DataModel.Mongo;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class ServiceProviderCreatedTemplate
    {
        [BsonId]
        public ObjectId TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string ServiceProviderId { get; set; } // Consider adding serviceprovider datetime?

        public Diagnosis Diagnosis { get; set; }

        public string Advice { get; set; }

        public List<Medicine> Medicines { get; set; }

        public bool IsDeleted { get; set; }
    }
}
