# Description

This library is designed to allow a developer to define and apply changes to a MongoDB database based on the current "schema" version of the database. Althought MongoDB is technically "schema-less", you may want to define indexes or pre-populate collections with data at application startup. Additionaly, you may want or need to apply some sort of document migration as your application evolves. This library was designed to handle these scenarios.

# Installation

Add a reference to the nuget package in your project using one of the methods below.

Visual Studio Package Manager Console:  
`Install-Package Stom.MongoDB.SchemaMigrations`

`dotnet` cli:  
`dotnet add package Stom.MongoDB.SchemaMigrations`

# Usage

1. Create migration classes derived from `SchemaVersion` and make sure to call the base constructor with the version string of what you want to apply to the database. Then override the `Apply()` method to perform any actions against the passed in `IMongoDatabase` object. The method should return `true` if successful, `false` otherwise.

    ```
        public class Version_0_1_0_migration : SchemaVersion
        {
            public Version_0_1_0_migration() : base("0.1.0")
            {

            }

            public async override Task<bool> Apply(IMongoDatabase db)
            {
                var index1 = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("i"));
                await db.GetCollection<BsonDocument>("MyCollection").Indexes.CreateOneAsync(index1);

                var index2 = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("i").Ascending("j"));
                await db.GetCollection<BsonDocument>("MyCollection2").Indexes.CreateOneAsync(index2);

                return true;
            }
        }
    ```
    ```
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
    ```
2. Create an instance of `SchemaMigrator` and add your migrations defined in step 1 to the `Migrations` collection
   ``` 
    IMongoClient mongo = new MongoClient();
    IMongoDatabase db = mongo.GetDatabase("SchemaMigrationTest");
            
    var migrator = new SchemaMigrator(db, new SchemaMigratorOptions(), serviceProvider.GetService<ILogger<SchemaMigrator>>());
    migrator.Migrations.Add(new Version(0, 1, 0), new Version_0_1_0_migration());
    migrator.Migrations.Add(new Version(0, 2, 0), new Version_0_2_0_migration());
   ```
3. Call `ApplyAll()` on the `SchemaMigrator` instance
   ```
   migrator.ApplyAll();
   ```

The `SchemaMigrator` will query the schema collection ("AppliedSchemas" by default but can be changed by setting the `CollectionName` property of `SchemaMigratorOptions`) to find the highest version applied. It will then loop through its list of migrations and apply only those that have a version newer than the highest version in the designated schema collection in the correct order. The result is of type `SchemaMigrationResult` and will contain the following information:

| Property | Type | Description |
| -------- | ---- | ----------- |
| `StartingVersion` | `Version` | The version from which migration is starting. This will be the latest version that exists in the *schema table* or "0.0.0" if none are found. |
| `EndingVersion` | `Version` | The version up to which the migration completed. |
| `MigrationsFound` | `int` | The number of migrations total that the migrator is aware of. |
| `MigrationsSkipped` | `int` | The number of migrations skipped due to already being applied. |
| `MigrationsApplied` | `int` | The number of migrations that were applied. |
| `ElapsedMilliseconds` | `long` | The number of milliseconds the entire migration process took. |


The *schema table* will contain a document for each version applied that consists of the following fields:

| Field | Type |
| ----- | ---- |
| _id   | `ObjectId` |
| version | `string` |
| dateApplied | `datetime` |