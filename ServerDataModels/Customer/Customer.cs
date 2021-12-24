using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServerDataModels.Common;
using System.Collections.Generic;

namespace ServerDataModels.Customer
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        [BsonId]
        public ObjectId CustomerId { get; set; }
        public List<CustomerProfile> Profiles { get; set; }
        public List<AuthInfo> AuthInfos { get; set; }
        public List<NotificationInfo> NotificationInfos { get; set; }
        public List<ServiceRequest.ServiceRequest> ServiceRequests { get; set; }
        public bool IsDeleted { get; set; }
    }
}
