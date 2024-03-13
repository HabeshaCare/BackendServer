using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.OptionsDTO
{
    public class FilterDTO
    {
        //Doctors Filter
        public int? MinYearExperience { get; set; }
        public int? MaxYearExperience { get; set; }
        public string? Specialization { get; set; }

        //InstitutionFilter
        public bool? Verified = null;
    }
}