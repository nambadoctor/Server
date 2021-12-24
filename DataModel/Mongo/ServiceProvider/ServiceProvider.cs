using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.Common;
using System.Collections.Generic;

namespace ServerDataModels.ServiceProvider
{
    [BsonIgnoreExtraElements]
    public class ServiceProvider
    {
        [BsonId]
        public ObjectId ServiceProviderId { get; set; }
        public List<AuthInfo> AuthInfos { get; set; }
        public List<ServiceProviderProfile> Profiles { get; set; }
        public List<NotificationInfo> NotificationInfos { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
        public List<ServiceProviderAvailability> Availabilities { get; set; }
        public bool IsDeleted { get; set; }
    }
}
