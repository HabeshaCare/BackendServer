using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserManagement.Models;

namespace UserManagement.Models
{
    public class Patient : User
    {
        [BsonRepresentation(BsonType.String)]
        public required string NationalId { get; set; }
        public int? Height { get; set; } = null;
        public int? Weight { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
    }
}