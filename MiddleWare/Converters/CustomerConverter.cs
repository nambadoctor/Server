using Client = DataModel.Client.Provider;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Converters
{
    public static class CustomerConverter
    {
        public static Mongo.CustomerProfile ConvertToMongoCustomerProfile(Client.CustomerProfile customerProfile)
        {
            var mongoCustomerProfile = new Mongo.CustomerProfile();

            mongoCustomerProfile.CustomerId = customerProfile.CustomerId;
            mongoCustomerProfile.FirstName = customerProfile.FirstName;
            mongoCustomerProfile.LastName = customerProfile.LastName;
            mongoCustomerProfile.Gender = customerProfile.Gender;
            mongoCustomerProfile.EmailAddress = customerProfile.EmailAddress;
            mongoCustomerProfile.OrganisationId = customerProfile.OrganisationId;
            mongoCustomerProfile.ServiceProviderId = customerProfile.ServiceProviderId;

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
        public static Client.CustomerProfile ConvertToClientCustomerProfile(Mongo.CustomerProfile customerProfile)
        {
            var clientCustomerProfile = new Client.CustomerProfile();

            clientCustomerProfile.CustomerId = customerProfile.CustomerId;
            clientCustomerProfile.FirstName = customerProfile.FirstName;
            clientCustomerProfile.LastName = customerProfile.LastName;
            clientCustomerProfile.Gender = customerProfile.Gender;
            clientCustomerProfile.EmailAddress = customerProfile.EmailAddress;
            clientCustomerProfile.OrganisationId = customerProfile.OrganisationId;
            clientCustomerProfile.ServiceProviderId = customerProfile.ServiceProviderId;

            if (customerProfile.DateOfBirth != null)
                clientCustomerProfile.DateOfBirth = ConvertToClientDateOfBirth(customerProfile.DateOfBirth);

            if (customerProfile.PhoneNumbers != null)
            {
                clientCustomerProfile.PhoneNumbers = new List<Client.PhoneNumber>();
                foreach (var ph in customerProfile.PhoneNumbers)
                {
                    clientCustomerProfile.PhoneNumbers.Add(ConvertToClientPhoneNumber(ph));
                }
            }

            return clientCustomerProfile;
        }

        public static List<Client.CustomerProfile> ConvertToClientCustomerProfileList(List<Mongo.CustomerProfile> customerList)
        {
            var clientCustomerProfiles = new List<Client.CustomerProfile>();

            foreach (var customer in customerList)
            {
                clientCustomerProfiles.Add(ConvertToClientCustomerProfile(customer));
            }

            return clientCustomerProfiles;
        }

        public static Client.DateOfBirth ConvertToClientDateOfBirth(Mongo.DateOfBirth dateOfBirth)
        {
            var clientDateOfBirth = new Client.DateOfBirth();

            clientDateOfBirth.DateOfBirthId = dateOfBirth.DateOfBirthId.ToString();

            clientDateOfBirth.Day = dateOfBirth.Day;
            clientDateOfBirth.Month = dateOfBirth.Month;
            clientDateOfBirth.Year = dateOfBirth.Year;

            return clientDateOfBirth;
        }

        public static Mongo.DateOfBirth ConvertToMongoDateOfBirth(Client.DateOfBirth dateOfBirth)
        {
            var mongoDateOfBirth = new Mongo.DateOfBirth();
            if (string.IsNullOrWhiteSpace(dateOfBirth.DateOfBirthId))
                mongoDateOfBirth.DateOfBirthId = MongoDB.Bson.ObjectId.GenerateNewId();
            else
                mongoDateOfBirth.DateOfBirthId = new MongoDB.Bson.ObjectId(dateOfBirth.DateOfBirthId);

            mongoDateOfBirth.Day = dateOfBirth.Day;
            mongoDateOfBirth.Month = dateOfBirth.Month;
            mongoDateOfBirth.Year = dateOfBirth.Year;

            return mongoDateOfBirth;
        }

        public static Client.PhoneNumber ConvertToClientPhoneNumber(Mongo.PhoneNumber phoneNumber)
        {
            var clientPhoneNumber = new Client.PhoneNumber();

            clientPhoneNumber.PhoneNumberId = phoneNumber.PhoneNumberId.ToString();

            clientPhoneNumber.Number = phoneNumber.Number;
            clientPhoneNumber.CountryCode = phoneNumber.CountryCode;
            clientPhoneNumber.Type = phoneNumber.Type;

            return clientPhoneNumber;
        }

        public static Mongo.PhoneNumber ConvertToMongoPhoneNumber(Client.PhoneNumber phoneNumber)
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
