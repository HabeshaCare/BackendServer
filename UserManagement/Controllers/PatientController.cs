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
            var response = await _patientService.GetPatientById(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}/profile")]
        [AuthorizeAccess]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] UpdatePatientDTO patientDTO)
        {
            var response = await _patientService.UpdatePatient(patientDTO, id);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPost("{id}/profile/upload-picture")]
        [AuthorizeAccess]
        public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        {
            try
            {
                var response = await _patientService.UploadProfilePic<UsagePatientDTO>(id, image);

                return new ObjectResult(response) { StatusCode = response.StatusCode };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }
    }
}