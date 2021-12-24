using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerDataModels.ServiceRequest
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
