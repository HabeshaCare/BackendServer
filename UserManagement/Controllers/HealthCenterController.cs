using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService.HealthCenterService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCenterController : ControllerBase
    {
        /*
        Todo: 
         - Get institution based on filter conditions from query. 
            1. Using id
            2. Using admin's id
            3. Filter verified institutions only
            4. Filter Unverified institutions only
            5. Pagination should be implemented by default
         - Create institution
         - Update institution
         - Delete institution   
        */

        private readonly IHealthCenterService _healthCenterService;
        public HealthCenterController(IHealthCenterService healthCenterService)
        {
            _healthCenterService = healthCenterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthCenters([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, healthCenters) = await _healthCenterService.GetHealthCenters(filterOptions, page, size);
            if (status == 0 || healthCenters.Length > 0)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institutions = healthCenters });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHealthCenter(string id)
        {
            var (status, message, healthCenter) = await _healthCenterService.GetHealthCenter(id);
            if (status == 0 || healthCenter == null)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institution = healthCenter });
        }

        [HttpPost]
        public async Task<IActionResult> AddHealthCenter([FromBody] HealthCenterDTO healthCenter)
        {
            var (status, message, createdHealthCenter) = await _healthCenterService.AddHealthCenter(healthCenter);
            if (status == 0 || createdHealthCenter == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = createdHealthCenter });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHealthCenterInfo([FromBody] UpdateHealthCenterDTO healthCenter, string id)
        {
            var (status, message, updatedHealthCenter) = await _healthCenterService.UpdateHealthCenter(healthCenter, id);
            if (status == 0 || updatedHealthCenter == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = updatedHealthCenter });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHealthCenterVerification([FromQuery] bool verified, string id)
        {
            var (status, message, healthCenter) = await _healthCenterService.UpdateInstitutionVerification<HealthCenter>(id, verified);
            if (status == 0 || healthCenter == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = healthCenter });
        }

        [HttpPost("{id}/upload-license")]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, healthCenter) = await _healthCenterService.UploadLicense<HealthCenter>(id, license);
            if (status == 0 || healthCenter == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = healthCenter });
        }
    }
}