using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ServerDataModels.ServiceProvider
{
    [BsonIgnoreExtraElements]
    public class PaymentDetails
    {
        [BsonId]
        public ObjectId PaymentDetailId { get; set; }
        public PaymentTransaction Transaction { get; set; }
        public double? PaidAmount { get; set; }
        public double? ServiceFee { get; set; }
        public bool? IsPaid { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType { get; set; }
    }
}
