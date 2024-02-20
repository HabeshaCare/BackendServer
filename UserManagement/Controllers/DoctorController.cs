using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Roles = "Normal, Doctor, Admin")]
        public async Task<IActionResult> GetDoctors([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, doctors) = await _doctorService.GetDoctors(filterOptions!, page, size);
            if (status == 0 || doctors == null)
                return NotFound(new { error = message });
            return Ok(new { users = doctors });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Normal, Doctor, Admin")]

        public async Task<IActionResult> GetDoctorById(string id)
        {
            var (status, message, doctor) = await _doctorService.GetDoctorById(id);
            if (status == 0 || doctor == null)
            {
                return NotFound(new { error = message });
            }
            return Ok(new { success = true });

        }

        /// <summary>
        /// Verify a doctor's credentials by an admin.
        /// </summary>
        [HttpPut("{id}/verify/")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyDoctor(string id)
        {
            var (status, message, doctor) = await _doctorService.VerifyDoctor(id);
            if (status == 0 || doctor == null)
                return BadRequest(new { errors = message });

            return Ok(new { message, user = doctor });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] UpdateDoctorDTO doctorDTO)
        {
            var (status, message, doctor) = await _doctorService.UpdateDoctor(doctorDTO, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = doctor });
        }

        /// <summary>
        /// Upload a license for a doctor.
        /// </summary>
        [HttpPost("{id}/license")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, doctor) = await _doctorService.UploadLicense(license, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = doctor });
        }
    }
}