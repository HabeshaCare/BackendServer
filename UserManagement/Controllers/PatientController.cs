using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Services.UserServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
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
        [AuthorizeAccess]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] UpdatePatientDTO patientDTO)
        {
            var (status, message, patient) = await _patientService.UpdatePatient(patientDTO, id);

            if (status == 0 || patient == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = patient });
        }

        [HttpPost("{id}/profile/upload-picture")]
        [AuthorizeAccess]
        public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        {
            try
            {
                var (status, message, user) = await _patientService.UploadProfilePic<Patient>(id, image);

                if (status == 0 || user == null)
                    return BadRequest(new { error = message });
                return Ok(new { message, user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }
    }
}