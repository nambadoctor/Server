namespace DataModel.Shared
{
    public static class ConnectionConfiguration
    {
        //Collections and constants
        public static string Secret_Mount_Path = "/mnt/secrets-store";
        public static string BlobContainerName = "ndreportscontainer";
        public static string ServiceProvideCollection = "ServiceProviders";
        public static string CustomerCollection = "Customers";
        public static string MedicineAutoFillCollection = "Medicines";
        public static string ServiceProviderCategoriesCollection = "ServiceProviderCategories";
        public static string BlockedUsersCollection = "BlockedUsers";
        public static string TestUsersCollection = "TesterUsers";
        public static string OrganisationCollection = "Organisations";
        public static string ServiceProviderCreatedTemplatesCollection = "ServiceProviderCreatedTemplates";
        public static string MongoDatabaseName = "NambaDoctorDb";

        //Production secrets
        public static string MongoConnectionString = "mongodb://nambadoctorppedb:jDqohESaSsmpS9aDusWioPZZpuJThfDSptQooRacoSB8GC6hLBnx1CwsxuYozN17bXpTgoUPdCO317OH7eur7w==@nambadoctorppedb.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@nambadoctorppedb@";
        public static string BlobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=grpctestcloudstore;AccountKey=u4a/w33QeNsoqjTNVZfiaJqHYMFBo3tti/780iHXKVAgu5wqHtLmKJg3BI7j5y0i5kRXCSSKQCk4K2QIEcV89Q==;EndpointSuffix=core.windows.net";
        public static string TwilioAccountSid = "ACf19d6ac6b8674474fb57038e79d1f452";
        public static string TwilioApiKey = "SKc4ce5414cb51748d8a59d9a41a28b92d";
        public static string TwilioApiSecret = "vLXFyK77BmvQU36Ujgp8H59TGIV7i5Rz";
        public static string FcmServerKey = "AAAAYXGR30E:APA91bGdKxoUY2cR39c9e3Cxf2xH4nIBCAA8EiKi3kqxARevqZ-ddPM1NiLXv8WBDJkScF57PTZz1sxeDLNv9uWLxQHur4CfM9D2pMB7V_HIm3p27nCdCmlvGsL9lJUPgPWw9sxxGicO";
        public static string FcmSenderId = "418517212993";
        public static string ApnBundleId = "nambadoctor.iOS.NambaDoctor";
        public static string ApnP8PrivateKey = "";
        public static string ApnP8PrivateKeyId = "58KXTN53NN";
        public static string ApnTeamId = "4H9M6DGZQ3";
        public static int ApnsServiceType = 0;
    }
}
