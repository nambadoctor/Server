using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Intake
    {
        [BsonId]
        public ObjectId IntakeId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Frequency Frequency { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Timings Timings { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IntakeInstruction Instruction { get; set; }
        public IntakeDosage IntakeDosage { get; set; }
    }
}
