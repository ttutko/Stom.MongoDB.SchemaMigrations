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
            using (var container = new Ductus.FluentDocker.Builders.Builder()
                .UseContainer()
                .UseImage("mongo:4-bionic")
                .ExposePort(27017, 27017)
                //.WithEnvironment("MONGO_INITDB_ROOT_USERNAME=root", "MONGO_INITDB_ROOT_PASSWORD=abc123")
                .WaitForPort("27017/tcp", 30000)
                .Build()
                .Start())
            {


                IMongoClient mongo = new MongoClient();
                IMongoDatabase db = mongo.GetDatabase("SchemaMigrationTest");

                try
                {
                    var sut = new SchemaMigrator(db, new SchemaMigratorOptions(), serviceProvider.GetService<ILogger<SchemaMigrator>>());
                    sut.Migrations.Add(new Version(0, 1, 0), new Version_0_1_0_migration());

                    var result = await sut.ApplyAll();

                    Assert.Equal(0, result.MigrationsSkipped);
                    Assert.Equal(1, result.MigrationsApplied);
                    Assert.Equal(1, result.MigrationsFound);
                }
                finally
                {
                    TeardownMongoDb();
                }
            }

        }

        [Fact(Skip="")]
        public async Task SchemaAtLatestVersionTest()
        {
            IMongoClient mongo = new MongoClient(new MongoClientSettings 
                { ConnectTimeout = TimeSpan.FromSeconds(2), ServerSelectionTimeout = TimeSpan.FromSeconds(2) });
            IMongoDatabase db = mongo.GetDatabase("SchemaMigrationTest");

            try
            {
                await db.GetCollection<SchemaDocument>("AppliedSchemas").InsertOneAsync(
                    new SchemaDocument
                    {
                        DateApplied = DateTime.Parse("2019-01-01"),
                        Version = "0.1.0"
                    }, null);

                var sut = new SchemaMigrator(db, new SchemaMigratorOptions(), serviceProvider.GetService<ILogger<SchemaMigrator>>());
                sut.Migrations.Add(new Version(0, 1, 0), new Version_0_1_0_migration());

                var result = await sut.ApplyAll();

                Assert.Equal(1, result.MigrationsSkipped);
                Assert.Equal(0, result.MigrationsApplied);
                Assert.Equal(1, result.MigrationsFound);
            }
            finally
            {
                TeardownMongoDb();
            }

        }

        [Fact(Skip="")]
        public async Task ApplyingMoreThanOneSchema()
        {
            IMongoClient mongo = new MongoClient(new MongoClientSettings 
                { ConnectTimeout = TimeSpan.FromSeconds(2), ServerSelectionTimeout = TimeSpan.FromSeconds(2) });
            IMongoDatabase db = mongo.GetDatabase("SchemaMigrationTest");

            try
            {
                //await db.GetCollection<SchemaDocument>("AppliedSchemas").InsertOneAsync(
                //    new SchemaDocument
                //    {
                //        DateApplied = DateTime.Parse("2019-01-01"),
                //        Version = "0.1.0"
                //    }, null);

                var sut = new SchemaMigrator(db, new SchemaMigratorOptions(), serviceProvider.GetService<ILogger<SchemaMigrator>>());
                sut.Migrations.Add(new Version(0, 1, 0), new Version_0_1_0_migration());
                sut.Migrations.Add(new Version(0, 2, 0), new Version_0_2_0_migration());

                var result = await sut.ApplyAll();

                Assert.Equal(0, result.MigrationsSkipped);
                Assert.Equal(2, result.MigrationsApplied);
                Assert.Equal(2, result.MigrationsFound);
            }
            finally
            {
                TeardownMongoDb();
            }

        }

        private void TeardownMongoDb()
        {
            IMongoClient mongo = new MongoClient();
            mongo.DropDatabase("SchemaMigrationTest");
        }

        public void SetupMongoDb()
        {
            // using(var container = new Ductus.FluentDocker.Builders.Builder()
            //     .UseContainer()
            //     .UseImage("mongo:4-bionic")
            //     .ExposePort(27017)
            //     .WithEnvironment("MONGO_INITDB_ROOT_USERNAME=root", "MONGO_INITDB_ROOT_PASSWORD=abc123")
            //     .WaitForPort("27017/tcp", 30000)
            //     .Build()
            //     .Start())
            //     {

            //     }

        }
    }
}
