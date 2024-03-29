﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class Appointment
    {
        [BsonId]
        public ObjectId AppointmentId { get; set; }
        public string ServiceRequestId { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceProviderName { get; set; }

        public string TreatmentPlanId { get; set; }
        public string TreatmentId { get; set; }
      
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }
        
        public string AppointmentReason { get; set; }
        public DateTime? ScheduledAppointmentStartTime { get; set; }
        public DateTime? ScheduledAppointmentEndTime { get; set; }
        public DateTime? ActualAppointmentStartTime { get; set; }
        public DateTime? ActualAppointmentEndTime { get; set; }
        public bool IsDeleted { get; set; }
        public Cancellation Cancellation { get; set; }

    }
}
