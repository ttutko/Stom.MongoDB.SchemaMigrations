using System;
using System.Collections.Generic;
using System.Text;

namespace Stom.MongoDB.SchemaMigrations
{
    public class SchemaMigratorOptions
    {
        public string CollectionName { get; set; }

        public SchemaMigratorOptions()
        {
            CollectionName = "AppliedSchemas";
        }
    }
}
