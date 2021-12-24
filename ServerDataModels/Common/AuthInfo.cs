using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDataModels.Common
{
    [BsonIgnoreExtraElements]
    public class AuthInfo
    {
        public ObjectId AuthInfoId { get; set; }

        public string AuthId { get; set; }

        public string AuthType { get; set; } //Right now its gonna be PhoneNumber

        public string AuthProviderGeneratedId { get; set; }
    }
}
