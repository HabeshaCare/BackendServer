using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserManagement.Models
{
    public class Administrator : User
    {
        [BsonRepresentation(BsonType.String)]
        public AdminRole AdminRole { get; set; }
    }
}