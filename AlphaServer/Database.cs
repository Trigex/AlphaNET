using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlphaServer
{
    class Database
    {
        MongoClient client;
        IMongoDatabase db;

        public async Task Init()
        {
            client = new MongoClient(AlphaServer.CONNECTION_STRING);
            db = client.GetDatabase(AlphaServer.DB);

            await db.CreateCollectionAsync("users");
    }
}
