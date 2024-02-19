using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserManagement.Models
{
    public class Institution
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string LicensePath { get; set; }
        public bool Verified { get; set; }
        public required InstitutionType Type { get; set; }
    }
}