using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DataModel.Mongo.Configuration;

[BsonIgnoreExtraElements]
public class SettingsConfiguration
{
    [BsonId]
    public ObjectId ConfigurationSettingId { get; set; }
    public string OrganisationId { get; set; }
    public string ServiceProviderId { get; set; }
    
    public AppointmentSettings AppointmentSettings { get; set; }
}