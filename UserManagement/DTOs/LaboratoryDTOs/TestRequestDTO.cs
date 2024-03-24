using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using UserManagement.Models;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class TestRequestDTO
    {
        public string? Id { get; set; }

        //This is the doctor Id
        public required string DoctorName { get; set; }

        //This is the laboratorian Id
        public string LaboratorianName { get; set; } = string.Empty;
        public required string LaboratoryName { get; set; }
        public DateTime RequestedDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}