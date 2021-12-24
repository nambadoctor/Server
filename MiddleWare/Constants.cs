namespace NambaMiddleWare
{
    public static class Constants
    { 
        public static class Auth
        {
            public static readonly string PhoneNumber = "phone_number";
            public static readonly string Header = "authorization";
            public static readonly string Spliter = " ";
            public static readonly List<string> anonymousAllowList 
                = new List<string>() {"/nd.v1.CustomerServiceProviderWorkerV1/GetServiceProviders"};

        }
    }
}
