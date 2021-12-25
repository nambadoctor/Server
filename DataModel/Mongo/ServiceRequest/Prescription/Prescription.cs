﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Prescription
    {
        [BsonId]
        public ObjectId PrescriptionId { get; set; }
        public List<Medicine> MedicineList { get; set; }
        public List<PrescriptionDocument> Documents { get; set; }
    }
}
