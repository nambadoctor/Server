using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class RouteOfAdministration
    {
        [BsonId]
        public ObjectId RouteOfAdministrationId { get; set; }
        public string Route { get; set; }
        public string Details { get; set; }
    }
}
