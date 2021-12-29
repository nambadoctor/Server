using Client = DataModel.Client.Provider;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Converters
{
    public static class CustomerConverter
    {
        public static Client.CustomerProfile ConvertToClientCustomerProfile(string customerId, Mongo.CustomerProfile customerProfile)
        {
            var clientCustomerProfile = new Client.CustomerProfile();

            clientCustomerProfile.CustomerId = customerId;
            clientCustomerProfile.FirstName = customerProfile.FirstName;
            clientCustomerProfile.LastName = customerProfile.LastName;
            clientCustomerProfile.Gender = customerProfile.Gender;
            clientCustomerProfile.DateOfBirth = customerProfile.DateOfBirth;

            var phone = customerProfile.PhoneNumbers.FirstOrDefault();
            if (phone != null)
                clientCustomerProfile.PhoneNumber = phone.CountryCode + phone.Number;

            return clientCustomerProfile;
        }

        public static List<Client.CustomerProfile> ConvertToClientCustomerProfileList(List<(string, Mongo.CustomerProfile)> customerList)
        {
            var clientCustomerProfiles = new List<Client.CustomerProfile>();

            foreach (var customer in customerList)
            {
                clientCustomerProfiles.Add(ConvertToClientCustomerProfile(customer.Item1, customer.Item2));
            }

            return clientCustomerProfiles;
        }
    }
}
