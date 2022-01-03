using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class ServiceRequest
    {
        [BsonId]
        public ObjectId ServiceRequestId { get; set; }
        public string CustomerId { get; set; }
        public string OrganisationId { get; set; } //For denormal
        public string ServiceProviderId { get; set; } // Consider adding serviceprovider datetime?
        public string AppointmentId { get; set; } //For denormal
        public string Reason { get; set; }
        public Examination Examination { get; set; }
        public List<Allergy> Allergies { get; set; }
        public List<History> Histories { get; set; }
        public Diagnosis Diagnosis { get; set; }
        public Vitals Vitals { get; set; }
        public List<AdditionalDetail> AdditionalDetails { get; set; }
        public List<Advice> Advices { get; set; }
        public List<Prescription> Prescriptions { get; set; } // Usually only 1 but multiple for history purpose
        public List<Report> Reports { get; set; }
        public bool IsDeleted { get; set; }
    }
}
