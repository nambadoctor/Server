using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientCommon = DataModel.Client.Provider.Common;
using Mongo = DataModel.Mongo;
using MongoDB.Bson;

namespace MiddleWare.Converters
{
    public static class CustomerConverter
    {
        public static Mongo.CustomerProfile ConvertToMongoCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfile)
        {
            var mongoCustomerProfile = new Mongo.CustomerProfile();

            mongoCustomerProfile.CustomerId = customerProfile.CustomerId;
            if (!string.IsNullOrWhiteSpace(customerProfile.CustomerProfileId))
                mongoCustomerProfile.CustomerProfileId = new ObjectId(customerProfile.CustomerProfileId);
            else
                mongoCustomerProfile.CustomerProfileId = ObjectId.GenerateNewId();
            mongoCustomerProfile.FirstName = customerProfile.FirstName;
            mongoCustomerProfile.LastName = customerProfile.LastName;
            mongoCustomerProfile.Gender = customerProfile.Gender;
            mongoCustomerProfile.OrganisationId = customerProfile.OrganisationId;

            if (customerProfile.DateOfBirth != null)
                mongoCustomerProfile.DateOfBirth = ConvertToMongoDateOfBirth(customerProfile.DateOfBirth);

            if (customerProfile.PhoneNumbers != null)
            {
                mongoCustomerProfile.PhoneNumbers = new List<Mongo.PhoneNumber>();
                foreach (var ph in customerProfile.PhoneNumbers)
                {
                    mongoCustomerProfile.PhoneNumbers.Add(ConvertToMongoPhoneNumber(ph));
                }
            }

            return mongoCustomerProfile;
        }
        public static ProviderClientOutgoing.OutgoingCustomerProfile ConvertToClientCustomerProfile(Mongo.CustomerProfile customerProfile)
        {
            var clientCustomerProfile = new ProviderClientOutgoing.OutgoingCustomerProfile();

            clientCustomerProfile.CustomerId = customerProfile.CustomerId;
            clientCustomerProfile.CustomerProfileId = customerProfile.CustomerProfileId.ToString();
            clientCustomerProfile.FirstName = customerProfile.FirstName;
            clientCustomerProfile.LastName = customerProfile.LastName;
            clientCustomerProfile.Gender = customerProfile.Gender;
            clientCustomerProfile.OrganisationId = customerProfile.OrganisationId;

            if (customerProfile.DateOfBirth != null)
                clientCustomerProfile.DateOfBirth = ConvertToClientDateOfBirth(customerProfile.DateOfBirth);

            if (customerProfile.PhoneNumbers != null)
            {
                clientCustomerProfile.PhoneNumbers = new List<ProviderClientCommon.PhoneNumber>();
                foreach (var ph in customerProfile.PhoneNumbers)
                {
                    clientCustomerProfile.PhoneNumbers.Add(ConvertToClientPhoneNumber(ph));
                }
            }

            return clientCustomerProfile;
        }

        public static List<ProviderClientOutgoing.OutgoingCustomerProfile> ConvertToClientCustomerProfileList(List<Mongo.CustomerProfile>? customerList)
        {
            var clientCustomerProfiles = new List<ProviderClientOutgoing.OutgoingCustomerProfile>();

            if (customerList != null)
                foreach (var customer in customerList)
                {
                    clientCustomerProfiles.Add(ConvertToClientCustomerProfile(customer));
                }

            return clientCustomerProfiles;
        }

        public static ProviderClientCommon.DateOfBirth ConvertToClientDateOfBirth(Mongo.DateOfBirth dateOfBirth)
        {
            var clientDateOfBirth = new ProviderClientCommon.DateOfBirth();

            clientDateOfBirth.DateOfBirthId = dateOfBirth.DateOfBirthId.ToString();

            clientDateOfBirth.Day = dateOfBirth.Day;
            clientDateOfBirth.Month = dateOfBirth.Month;
            clientDateOfBirth.Year = dateOfBirth.Year;
            clientDateOfBirth.Age = dateOfBirth.Age;

            return clientDateOfBirth;
        }

        public static Mongo.DateOfBirth ConvertToMongoDateOfBirth(ProviderClientCommon.DateOfBirth dateOfBirth)
        {
            var mongoDateOfBirth = new Mongo.DateOfBirth();
            if (string.IsNullOrWhiteSpace(dateOfBirth.DateOfBirthId))
                mongoDateOfBirth.DateOfBirthId = MongoDB.Bson.ObjectId.GenerateNewId();
            else
                mongoDateOfBirth.DateOfBirthId = new MongoDB.Bson.ObjectId(dateOfBirth.DateOfBirthId);

            mongoDateOfBirth.Day = dateOfBirth.Day;
            mongoDateOfBirth.Month = dateOfBirth.Month;
            mongoDateOfBirth.Year = dateOfBirth.Year;

            mongoDateOfBirth.Age = dateOfBirth.Age;
            mongoDateOfBirth.CreatedDate = dateOfBirth.CreatedDate;

            return mongoDateOfBirth;
        }

        public static ProviderClientCommon.PhoneNumber ConvertToClientPhoneNumber(Mongo.PhoneNumber phoneNumber)
        {
            var clientPhoneNumber = new ProviderClientCommon.PhoneNumber();

            clientPhoneNumber.PhoneNumberId = phoneNumber.PhoneNumberId.ToString();

            clientPhoneNumber.Number = phoneNumber.Number;
            clientPhoneNumber.CountryCode = phoneNumber.CountryCode;
            clientPhoneNumber.Type = phoneNumber.Type;

            return clientPhoneNumber;
        }

        public static Mongo.PhoneNumber ConvertToMongoPhoneNumber(ProviderClientCommon.PhoneNumber phoneNumber)
        {
            var mongoPhoneNumber = new Mongo.PhoneNumber();

            if (string.IsNullOrWhiteSpace(phoneNumber.PhoneNumberId))
                mongoPhoneNumber.PhoneNumberId = MongoDB.Bson.ObjectId.GenerateNewId();
            else
                mongoPhoneNumber.PhoneNumberId = new MongoDB.Bson.ObjectId(phoneNumber.PhoneNumberId);

            mongoPhoneNumber.Number = phoneNumber.Number;
            mongoPhoneNumber.CountryCode = phoneNumber.CountryCode;
            mongoPhoneNumber.Type = phoneNumber.Type;

            return mongoPhoneNumber;
        }
    }
}
