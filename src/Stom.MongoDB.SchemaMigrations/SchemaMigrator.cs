using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Stom.MongoDB.SchemaMigrations
{
    public class SchemaMigrator
    {
        private readonly IMongoDatabase db;
        private readonly SchemaMigratorOptions options;
        private readonly ILogger<SchemaMigrator> logger;

        public SortedList<Version, SchemaVersion> Migrations { get; private set; }

        public SchemaMigrator(IMongoDatabase db, SchemaMigratorOptions options, ILogger<SchemaMigrator> logger)
        {
            this.db = db;
            this.options = options;
            this.logger = logger;
            Migrations = new SortedList<Version, SchemaVersion>();
        }

        public async System.Threading.Tasks.Task ApplyAll()
        {
            int applied = 0, skipped = 0;

            Stopwatch sw = new Stopwatch();
            logger.LogInformation($"Beginning [ApplyAll] action for {Migrations.Count} migrations...");
            sw.Start();

            var latestVersion = new Version(0, 0, 0);

            var latestSchemaAppliedDoc = (await GetAppliedVersionsAsync()).LastOrDefault();
            if(!latestSchemaAppliedDoc.Equals(default(KeyValuePair<Version, SchemaDocument>)))
            {
                latestVersion = latestSchemaAppliedDoc.Key;
            }

            foreach(var curVersion in Migrations)
            {
                if(curVersion.Key <= latestVersion)
                {
                    logger.LogInformation($"Migrations for version {curVersion.Key} skipped as newer version ({latestVersion}) is already applied.");
                    skipped += 1;
                    continue;
                }

                logger.LogInformation($"Applying migrations for {curVersion.Key.ToString()}");

                await curVersion.Value.Apply(db);

                logger.LogInformation($"Done applying migrations for {curVersion.Key.ToString()}");
            }

            sw.Stop();
            logger.LogInformation($"Completed [ApplyAll] action in {sw.ElapsedMilliseconds}ms. Applied: {applied}, Skipped: {skipped}");
        }

        public void ApplyToVersion(string version)
        {

        }

        private async System.Threading.Tasks.Task<SortedList<Version, SchemaDocument>> GetAppliedVersionsAsync()
        {
            SortedList<Version, SchemaDocument> sortedVersions = new SortedList<Version, SchemaDocument>();
            var filter = Builders<SchemaDocument>.Filter.Empty;

            var allVersions = await db.GetCollection<SchemaDocument>(options.CollectionName).Find(filter).ToListAsync();

            foreach (var ver in allVersions)
            {
                sortedVersions.Add(Version.Parse(ver.Version), ver);
            }

            return sortedVersions;
        }
    }
}
