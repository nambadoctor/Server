using MongoDB.Driver;
using DataModel.Mongo;
using DataModel.Shared;

namespace DataLayer
{
    public sealed class MongoDBInstance
    {
        IMongoDatabase mongoDb = null;
        MongoClient mongoClient = null;

        private static readonly object padlock = new object();
        private static MongoDBInstance mongoDbInstance = null;

        public static MongoDBInstance Instance
        {
            get
            {
                lock (padlock)
                {
                    if (mongoDbInstance == null)
                    {
                        mongoDbInstance = new MongoDBInstance();
                    }
                    return mongoDbInstance;
                }
            }
        }
        public MongoDBInstance()
        {
            mongoClient = new MongoClient(ConnectionConfiguration.MongoConnectionString);
            this.mongoDb = mongoClient.GetDatabase(ConnectionConfiguration.MongoDatabaseName);
        }

        public IMongoDatabase GetMongoDB()
        {
            return this.mongoDb;
        }

        public MongoClient GetMongoClient()
        {
            return this.mongoClient;
        }
    }
}
