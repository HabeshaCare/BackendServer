using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService.HealthCenterService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HealthCenterController : ControllerBase
    {
        /*
        Todo: 
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
            var response = await _healthCenterService.GetHealthCenters(filterOptions, page, size);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHealthCenter(string id)
        {
            var response = await _healthCenterService.GetHealthCenter(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPost]
        [Authorize(Roles = "HealthCenterAdmin, SuperAdmin")]
        public async Task<IActionResult> AddHealthCenter([FromBody] HealthCenterDTO healthCenter)
        {
            string adminId = HttpContext.Items["UserId"]?.ToString() ?? "";
            var response = await _healthCenterService.AddHealthCenter(healthCenter, adminId);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPut("{id}")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UpdateHealthCenterInfo([FromBody] UpdateHealthCenterDTO healthCenter, string id)
        {
            var response = await _healthCenterService.UpdateHealthCenter(healthCenter, id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPut("{id}/verify")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateHealthCenterVerification([FromQuery] bool verified, string id)
        {
            var response = await _healthCenterService.UpdateInstitutionVerification<HealthCenter>(id, verified);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPost("{id}/upload-license")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var response = await _healthCenterService.UploadLicense<HealthCenter>(id, license);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }
    }
}