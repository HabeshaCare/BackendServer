using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService;

namespace UserManagement.Controllers
{
    [ApiController]
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
            var (status, message, laboratories) = await _laboratoryService.GetLaboratories(filterOptions, page, size);
            if (status == 0 || laboratories.Length > 0)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institutions = laboratories });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaboratory(string id)
        {
            var (status, message, laboratory) = await _laboratoryService.GetLaboratory(id);
            if (status == 0 || laboratory == null)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institution = laboratory });
        }

        [HttpPost]
        public async Task<IActionResult> AddLaboratory([FromBody] LaboratoryDTO laboratory)
        {
            var (status, message, createdLaboratory) = await _laboratoryService.AddLaboratory(laboratory);
            if (status == 0 || createdLaboratory == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = createdLaboratory });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLaboratoryInfo([FromBody] UpdateLaboratoryDTO laboratory, string id)
        {
            var (status, message, updatedLaboratory) = await _laboratoryService.UpdateLaboratory(laboratory, id);
            if (status == 0 || updatedLaboratory == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = updatedLaboratory });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLaboratoryVerification([FromQuery] bool verified, string id)
        {
            var (status, message, laboratory) = await _laboratoryService.UpdateInstitutionVerification<Laboratory>(id, verified);
            if (status == 0 || laboratory == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = laboratory });
        }

        [HttpPost("{id}/upload-license")]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, laboratory) = await _laboratoryService.UploadLicense<Laboratory>(id, license);
            if (status == 0 || laboratory == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = laboratory });
        }
    }
}