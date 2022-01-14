using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataModel.Mongo;
using System.Collections.Generic;

namespace DataModel.Mongo
{
    [BsonIgnoreExtraElements]
    public class ServiceProviderProfile
    {
        [BsonId]
        public ObjectId ServiceProviderProfileId { get; set; }
        public string ServiceProviderId { get; set; }
        public string OrganisationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ServiceProviderType { get; set; }
        public string Gender { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<Address> Addresses { get; set; }
        public string EmailAddress { get; set; }
        public string ProfilePictureUrl { get; set; }
        public AdditionalInfo AdditionalInfo { get; set; }
        public List<string> Languages { get; set; }
        public List<Education> Educations { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public string RegistrationNumber { get; set; }
        public int AppointmentDuration { get; set; }
        public List<string> Roles { get; set; }

    }
}
