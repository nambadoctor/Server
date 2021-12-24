using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDataModels.ServiceProvider
{
    [BsonIgnoreExtraElements]
    public class PaymentTransaction
    {
        [BsonId]
        public ObjectId PaymentTransactionId { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string GatewayName { get; set; }
        public string GatewayGeneratedId { get; set; }
        public List<string> Notes { get; set; }
    }
}
