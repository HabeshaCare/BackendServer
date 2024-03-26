using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace UserManagement.Models
{
    public class TestRequest
    {
        public string? Id { get; set; }
        //This is the doctor Id
        public required string RequestorId { get; set; }

        //This is the laboratorian Id
        public string HandlerId { get; set; } = string.Empty;
        public required string LaboratoryId { get; set; }
        public DateTime RequestedDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

    }
}