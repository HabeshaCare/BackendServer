using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UserManagement.Models
{
    public class TestRequest
    {
        //This is the doctor Id
        public required string RequestorId { get; set; }

        //This is the laboratorian Id
        public required string HandlerId { get; set; }
        public DateTime RequestedDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public required RequestStatus Status { get; set; }
    }
}