using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserManagement.Models
{
    public class Doctor : User
    {
        [BsonRepresentation(BsonType.String)]
        public string? LicensePath { get; set; }
        public string Specialization { get; set; } = "Medical";
        public int? YearOfExperience { get; set; }
        public string AssociatedHealthCenterId { get; set; } = string.Empty;
        public bool? Verified { get; set; } = false;
    }
}