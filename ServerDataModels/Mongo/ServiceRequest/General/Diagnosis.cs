using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDataModels.ServiceRequest
{
    [BsonIgnoreExtraElements]
    public class Diagnosis
    {
        public string Name { get; set; }
        public string DiagnosisType { get; set; } //Provisional, Definitive
    }
}
