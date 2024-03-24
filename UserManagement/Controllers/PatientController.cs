using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Services.UserServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize(Roles = "Patient, SuperAdmin, HealthCenterAdmin, Reception, Doctor")]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("{id}/profile")]
        [AuthorizePatientAccess]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var response = await _patientService.GetPatientById(id);
            SResponseDTO<UsageUserDTO>? updatedResponse = null;
            var role = HttpContext.Items["Role"]?.ToString() ?? UserRole.Reception.ToString();
            switch (role)
            {
                case "Reception":
                    updatedResponse = new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data as UsageUserDTO, Errors = response.Errors, Success = response.Success };
                    break;
                default:
                    break;
            }

            if (updatedResponse != null)
                return new ObjectResult(updatedResponse) { StatusCode = updatedResponse.StatusCode };
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}/profile")]
        [AuthorizePatientAccess]
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