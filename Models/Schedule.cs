using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAuthentication.Models
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        public bool Confirmed { get; set; }
        public required string SchedulerId { get; set; }
        public required string DoctorId { get; set; }
    }
}