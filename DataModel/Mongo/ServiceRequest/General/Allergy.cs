using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Allergy
    {
        [BsonId]
        public ObjectId AllergyId { get; set; }
        public string AllergyName { get; set; }
    }
}
