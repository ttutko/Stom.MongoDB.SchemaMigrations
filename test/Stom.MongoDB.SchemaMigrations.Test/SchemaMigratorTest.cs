using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Stom.MongoDB.SchemaMigrations.Test
{
    public class SchemaMigratorTest
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        ServiceProvider serviceProvider;
        public SchemaMigratorTest()
        {
            serviceCollection.AddLogging(c => c.AddConsole());

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task NoSchemaCollectionTest()
        {
            IMongoClient mongo = new MongoClient();
            IMongoDatabase db = mongo.GetDatabase("SchemaMigrationTest");

            var sut = new SchemaMigrator(db, new SchemaMigratorOptions(), serviceProvider.GetService<ILogger<SchemaMigrator>>());
            sut.Migrations.Add(new Version(0, 1, 0), new Version_0_1_0_migration());

            await sut.ApplyAll();

            throw new NotImplementedException();
        }
    }
}
