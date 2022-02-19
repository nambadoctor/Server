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
        public List<PrescriptionDocument> PrescriptionDocuments { get; set; }
        public List<Report> Reports { get; set; }
        public List<Note> Notes { get; set; }
        public string TreatmentId { get; set; }
        public string TreatmentPlanId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
