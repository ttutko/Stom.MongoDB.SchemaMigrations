using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stom.MongoDB.SchemaMigrations
{
    public abstract class SchemaVersion
    {
        private Version version;

        public SchemaVersion(string version)
        {
            this.version = Version.Parse(version);
        }

        public abstract bool Apply(IMongoDatabase db);
    }
}
