using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserManagement.Models
{
    public class LabTest
    {
        [BsonId]
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public required string TestName { get; set; }
        public string? TestValue { get; set; }

    }
}