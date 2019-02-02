using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Stom.MongoDB.SchemaMigrations.Test
{
    public class Version_0_1_0_migration : SchemaVersion
    {
        public Version_0_1_0_migration() : base("0.1.0")
        {

        }

        public async override Task<bool> Apply(IMongoDatabase db)
        {
            //db.CreateCollection("MyCollection");

            var index1 = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("i"));
            await db.GetCollection<BsonDocument>("MyCollection").Indexes.CreateOneAsync(index1);

            var index2 = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("i").Ascending("j"));
            await db.GetCollection<BsonDocument>("MyCollection2").Indexes.CreateOneAsync(index2);

            return true;
        }
    }
}
