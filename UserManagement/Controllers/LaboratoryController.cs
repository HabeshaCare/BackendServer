using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LaboratoryController : ControllerBase
    {
        private readonly ILaboratoryService _laboratoryService;
        public LaboratoryController(ILaboratoryService laboratoryService)
        {
            _laboratoryService = laboratoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLaboratories([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _laboratoryService.GetLaboratories(filterOptions, page, size);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaboratory(string id)
        {
            var response = await _laboratoryService.GetLaboratory(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPost]
        [Authorize(Roles = "LaboratoryAdmin, HealthCenter, SuperAdmin")]
        public async Task<IActionResult> AddLaboratory([FromBody] LaboratoryDTO laboratory)
        {
            string adminId = HttpContext.Items["UserId"]?.ToString() ?? "";
            var response = await _laboratoryService.AddLaboratory(laboratory, adminId);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UpdateLaboratoryInfo([FromBody] UpdateLaboratoryDTO laboratory, string id)
        {
            var response = await _laboratoryService.UpdateLaboratory(laboratory, id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPut("{id}/verify")]
        [Authorize(Roles = "HealthCenterAdmin, SuperAdmin")]
        public async Task<IActionResult> UpdateLaboratoryVerification([FromQuery] bool verified, string id)
        {
            var response = await _laboratoryService.UpdateInstitutionVerification<Laboratory>(id, verified);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }

        [HttpPost("{id}/upload-license")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var response = await _laboratoryService.UploadLicense<Laboratory>(id, license);
            return new ObjectResult(response) { StatusCode = response.StatusCode };

        }
    }
}