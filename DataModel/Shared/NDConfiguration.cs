namespace DataModel.Shared
{
    public class MongoConfiguration
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
            
    }
    public class TwillioConfiguration
    {
        public string AccountSid { get; set; }
        public string ApiKey { get; set; }  
        public string ApiSecret { get; set; }

    }
    public class FCMConfiguration
    {
        public string ServerKey { get; set; }   
        public string SenderId { get; set; }

    }

    public class APNConfiguration
    {
        public int ServiceType { get; set; }
        public string BundleId { get; set; }
        public string P8PrivateKey { get; set; }
        public string P8PrivateKeyId { get; set; }
        public string TeamId { get; set; }

    }

    public class BlobStorageConfiguration
    {
        public string ConnectionString { get; set; }

    }

}
