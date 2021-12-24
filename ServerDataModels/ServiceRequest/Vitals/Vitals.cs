using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Vitals
    {
        [BsonId]
        public ObjectId VitalsId { get; set; }
        public Height Height { get; set; }
        public Weight Weight { get; set; }
        public BloodPressure BloodPressure { get; set; }
        public BloodSugar BloodSugar { get; set; }
        public Habit BadHabit { get; set; }
        public Pulse Pulse { get; set; }
        public RespiratoryRate RespiratoryRate { get; set; }
        public Temperature Temperature { get; set; }
        public Saturation Saturation { get; set; }
    }
}