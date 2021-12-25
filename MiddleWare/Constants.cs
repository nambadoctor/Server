namespace NambaMiddleWare
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
    }
}
