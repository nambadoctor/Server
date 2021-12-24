using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.ServiceRequest;
using System.Collections.Generic;

namespace ServerDataModels.Other
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
