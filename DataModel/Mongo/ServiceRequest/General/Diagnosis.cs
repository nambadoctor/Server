using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Diagnosis
    {
        public string Name { get; set; }
        public string DiagnosisType { get; set; } //Provisional, Definitive
    }
}
