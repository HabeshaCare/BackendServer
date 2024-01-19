using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models.DTOs.OptionsDTO;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.UserServices;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    [Authorize(Roles = "Normal, Admin")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors([FromQuery] DoctorFilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, doctors) = await (filterOptions != null ? _doctorService.GetDoctors(filterOptions, page, size) : _doctorService.GetDoctors(page, size));
            if (status == 0 || doctors == null)
                return NotFound(new { error = message });
            return Ok(new { users = doctors });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var (status, message, doctor) = await _doctorService.GetDoctorById(id);
            if (status == 0 || doctor == null)
            {
                return NotFound(new { error = message });
            }
            return Ok(new { user = doctor });

        }

        [HttpPut("verify/{id}")]
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
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorDTO doctorDTO, String id)
        {
            var (status, message, doctor) = await _doctorService.Update(doctorDTO, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = doctor });
        }
    }
}