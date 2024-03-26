using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UserManagement.Models;

namespace UserManagement.DTOs.MessageDTOs
{
    public class MessageDTO
    {

        public required string Content { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public MessageType Type { get; set; } = MessageType.Human;
        public required string UserId { get; set; }
    }
}