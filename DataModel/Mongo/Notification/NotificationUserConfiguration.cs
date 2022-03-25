using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataModel.Mongo.Notification
{
    [BsonIgnoreExtraElements]
    public class NotificationUserConfiguration
    {
        [BsonId]
        public ObjectId UserConfigurationId { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        
        public string Name { get; set; }

        public List<NotificationSubscription> SubscribedNotifications;
        
        public DateTime CreatedDateTime { get; set; }
    }
}
