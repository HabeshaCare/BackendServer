using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using UserManagement.Models;

namespace UserManagement.DTOs.InstitutionDTOs
{
    public class InstitutionDTO
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string LicensePath { get; set; }
        public bool Verified { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required InstitutionType Type { get; set; }
    }
}