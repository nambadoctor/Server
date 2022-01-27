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
        public string Name { get; set; }
        public string OrginalInstructions { get; set; } // Imutable

        public string ActualProcedure { get; set; } // Mostly this is copy of OriginalInstruction

        public DateTime PlannedDateTime { get; set; }
        public DateTime ActualDateTime { get; set; }
        public string TreatmentInstanceServiceRequestId { get; set; } // This is servicerequestId when the treatment happened

        public DateTime CreatedDateTime { get; set; }


        public string TreatmentInstanceAppointmentId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TreatmentStatus Status { get; set; } // Mutable
    }
}
