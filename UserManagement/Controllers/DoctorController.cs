using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Controllers
{
    /// <summary>
    /// Controller responsible for handling doctor-related operations.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Get a list of doctors based on optional filter options, page, and size.
        /// </summary>
        /// <param name="filterOptions">Filter options for doctor retrieval (optional).</param>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="size">Number of items per page (default is 10).</param>
        /// <returns>ActionResult containing the list of doctors.</returns>
        [HttpGet]
        [Authorize(Roles = "Patient, Doctor, SuperAdmin, HealthCenterAdmin, PharmacyAdmin, LaboratoryAdmin, Reception")]
        public async Task<IActionResult> GetDoctors([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, doctors) = await _doctorService.GetDoctors(filterOptions!, page, size);
            if (status == 0 || doctors == null)
                return NotFound(new { error = message });
            return Ok(new { users = doctors });
        }

        [HttpGet("{id}/profile")]
        [Authorize(Roles = "Patient, Doctor, SuperAdmin, HealthCenterAdmin, PharmacyAdmin, LaboratoryAdmin, Reception")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var (status, message, doctor) = await _doctorService.GetDoctorById(id);
            if (status == 0 || doctor == null)
            {
                return NotFound(new { error = message });
            }
            return Ok(new { success = true, user = doctor });
        }

        /// <summary>
        /// Verify a doctor's credentials by an admin.
        /// </summary>
        [HttpPut("{id}/verify/")]
        [Authorize(Roles = "HealthCenterAdmin")]
        public async Task<IActionResult> VerifyDoctor(string id)
        {
            var (status, message, doctor) = await _doctorService.VerifyDoctor(id);
            if (status == 0 || doctor == null)
                return BadRequest(new { errors = message });

            return Ok(new { message, user = doctor });
        }

        [HttpPut("{id}/profile")]
        [Authorize(Roles = "Doctor")]
        [AuthorizeAccess]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] UpdateDoctorDTO doctorDTO)
        {
            var (status, message, doctor) = await _doctorService.UpdateDoctor(doctorDTO, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = doctor });
        }

        [HttpPost("{id}/profile/upload-picture")]
        [AuthorizeAccess]
        public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        {
            try
            {
                var (status, message, user) = await _doctorService.UploadProfilePic<UsageDoctorDTO>(id, image);

                if (status == 0 || user == null)
                    return BadRequest(new { error = message });
                return Ok(new { message, user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }

        /// <summary>
        /// Upload a license for a doctor.
        /// </summary>
        [HttpPost("{id}/profile/upload-license")]
        [Authorize(Roles = "Doctor")]
        [AuthorizeAccess]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, doctor) = await _doctorService.UploadLicense(license, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = doctor });
        }
    }
}