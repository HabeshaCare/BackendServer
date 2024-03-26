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
    public class Administrator : User
    {
        //The institution id managed by the administrator
        public string InstitutionId { get; set; } = string.Empty;
        public string AssociatedHealthCenterId { get; set; } = string.Empty;
        public bool Verified { get; set; }
    }
}