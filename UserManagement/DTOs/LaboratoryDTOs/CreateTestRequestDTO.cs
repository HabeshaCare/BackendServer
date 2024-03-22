using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using UserManagement.Models;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class CreateTestRequestDTO
    {

        //This is the doctor Id
        public required string RequestorId { get; set; }
        //This is the laboratorian Id
        public required string HandlerId { get; set; }
        public required string LaboratoryId { get; set; }
        public DateTime RequestedDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}