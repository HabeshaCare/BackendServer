using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.UserServices;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors([FromQuery] int page=1, [FromQuery] int size=10)
        {
            var (status, message, doctors) = await _doctorService.GetDoctors(page, size);
            if (status == 0 || doctors == null)
            {
                return BadRequest(new{error=message});
            }
            return Ok(new{users=doctors});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var (status, message, doctor) = await _doctorService.GetDoctorById(id);
            if (status == 0 || doctor == null)
            {
                return NotFound(new{error=message});
            }
            return Ok(new{user=doctor});

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorDTO doctorDTO, String id)
        {
            var (status, message, doctor) = await _doctorService.Update(doctorDTO, id);

            if (status == 0 || doctor == null)
                return BadRequest(new { error = message });

            return Ok(new {message, user = doctor });
        }
    }
}