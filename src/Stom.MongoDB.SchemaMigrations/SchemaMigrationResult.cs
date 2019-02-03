using System;
using System.Collections.Generic;
using System.Text;

namespace Stom.MongoDB.SchemaMigrations
{
    public class SchemaMigrationResult
    {
        public Version StartingVersion { get; set; }
        public Version EndingVersion { get; set; }
        public int MigrationsFound { get; set; }
        public int MigrationsSkipped { get; set; }
        public int MigrationsApplied { get; set; }
        public long ElapsedMiliseconds { get; set; }
    }
}
