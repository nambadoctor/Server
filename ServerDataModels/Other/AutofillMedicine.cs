using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.Other
{
    [BsonIgnoreExtraElements]
    public class AutofillMedicine
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public string Ingredients { get; set; }

        public int IngredientCount { get; set; }

        public string BrandName { get; set; }

        public string RouteOfAdministration { get; set; }

        public string DrugType { get; set; }

        public string CompanyName { get; set; }

        public bool IsVerified { get; set; }

    }
}
