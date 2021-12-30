using Client = DataModel.Client.Provider;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Converters
{
    public static class CustomerConverter
    {
        public static Client.CustomerProfile ConvertToClientCustomerProfile(Mongo.CustomerProfile customerProfile)
        {
            var clientCustomerProfile = new Client.CustomerProfile();

            clientCustomerProfile.CustomerId = customerProfile.CustomerId;
            clientCustomerProfile.FirstName = customerProfile.FirstName;
            clientCustomerProfile.LastName = customerProfile.LastName;
            clientCustomerProfile.Gender = customerProfile.Gender;
            clientCustomerProfile.DateOfBirth = $"{customerProfile.DateOfBirth.Day}/{customerProfile.DateOfBirth.Month}{customerProfile.DateOfBirth.Year}";

            var phone = customerProfile.PhoneNumbers.FirstOrDefault();
            if (phone != null)
                clientCustomerProfile.PhoneNumber = phone.CountryCode + phone.Number;

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
    }
}
