using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Treatment
    {
        [BsonId]
        public ObjectId TreatmentId { get; set; }
        public TreatmentDetail TreatmentDetail { get; set; }
        public DateTime PlannedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string AppointmentId { get; set; }
        public string ServiceRequestId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TreatmentStatus Status { get; set; }
    }
}
