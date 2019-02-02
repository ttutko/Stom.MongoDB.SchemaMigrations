using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stom.MongoDB.SchemaMigrations
{
    public abstract class SchemaVersion
    {
        private Version version;

        public SchemaVersion(string version)
        {
            this.version = Version.Parse(version);
        }

        public abstract Task<bool> Apply(IMongoDatabase db);
    }
}
