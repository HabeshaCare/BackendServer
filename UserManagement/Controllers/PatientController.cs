using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Services.UserServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var (status, message, patient) = await _patientService.GetPatientById(id);
            if (status == 0 || patient == null)
            {
                return NotFound(new { error = message });
            }
            return Ok(new { success = true, user = patient });
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] UpdatePatientDTO patientDTO)
        {
            var (status, message, patient) = await _patientService.UpdatePatient(patientDTO, id);

            if (status == 0 || patient == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = patient });
        }
    }
}