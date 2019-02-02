using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stom.MongoDB.SchemaMigrations
{
    public class SchemaMigrator
    {
        private readonly IMongoDatabase db;

        public SchemaMigrator(IMongoDatabase db)
        {
            this.db = db;
        }
    }
}
