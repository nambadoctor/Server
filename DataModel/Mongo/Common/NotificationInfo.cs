using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class NotificationInfo
    {
        [BsonId]
        public ObjectId NotificationInfoId { get; set; }
        public string DeviceType { get; set; }//Android, iOS, Web
        public string NotificationProvider { get; set; }// Firebase, Apple apn
        public string DeviceToken { get; set; } //This is a string in both providers
        public string OrganisationId { get; set; }
    }
}
