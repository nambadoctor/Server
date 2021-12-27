using System.Collections.Generic;

namespace DataModel.Shared
{
    public static class Constants
    { 
        public static class Auth
        {
            public static readonly string PhoneNumber = "phone_number";
            public static readonly string Header = "Authorization";
            public static readonly string Spliter = " ";
            public static readonly List<string> anonymousAllowList 
                = new List<string>() {"/ServiceProviders HTTP: GET"};

        }
        public static class MongoDB
        {
            public static string ServiceProvideCollection = "ServiceProviders";
            public static string CustomerCollection = "Customers";
            public static string MedicineAutoFillCollection = "Medicines";
            public static string ServiceProviderCategoriesCollection = "ServiceProviderCategories";
            public static string BlockedUsersCollection = "BlockedUsers";
            public static string TestUsersCollection = "TesterUsers";
            public static string OrganisationCollection = "Organisations";
            public static string ServiceProviderCreatedTemplatesCollection = "ServiceProviderCreatedTemplates";
        }
        public static class Firebase
        {
            public  static string FirebaseProjectId = "ds-connect";

        }
    }
}
