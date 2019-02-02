using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stom.MongoDB.SchemaMigrations
{
    public class SchemaDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Version { get; set; }
        public DateTime DateApplied { get; set; }
    }
}
