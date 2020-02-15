using System;

namespace MongoHelper.xUnitTests
{
    public class MongoFixture : IDisposable
    {
        private const string ConnStr = "mongodb://127.0.0.1:27017";
        private const string DbName = "test";
        public string Collection { get; } = "persons";

        public MongoHelper Mongo { get; set; }

        public MongoFixture()
        {
            var jsons = new string[]
            {
                "{Name:'Colin',Age:15}",
                "{Name:'Robin',Age:20}",
                "{Name:'Jim',Age:25}",
                "{Name:'Tom',Age:16}",
                "{Name:'Bob',Age:25}",
                "{Name:'Jerry',Age:28}",
                "{Name:'Chris',Age:28}"
            };

            Mongo = new MongoHelper(ConnStr, DbName);
            Mongo.InsertAsync(Collection, jsons).Wait();
        }

        public async void Dispose()
        {
            //await Mongo.DropCollection(Collection);
            await Mongo.DropDatabaseAsync(DbName);
        }
    }
}
