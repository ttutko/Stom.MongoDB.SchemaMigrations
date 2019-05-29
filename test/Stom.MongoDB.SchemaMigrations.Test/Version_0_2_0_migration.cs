using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Stom.MongoDB.SchemaMigrations.Test
{
    public class Version_0_2_0_migration : SchemaVersion
    {
        public Version_0_2_0_migration() : base("0.2.0")
        {

        }

        public override async Task<bool> Apply(IMongoDatabase db)
        {
            var index1 = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("i").Ascending("field1"));
            await db.GetCollection<BsonDocument>("MyCollection").Indexes.CreateOneAsync(index1);

            return true;
        }
    }
}
