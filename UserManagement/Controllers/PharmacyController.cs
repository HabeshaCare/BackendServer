using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        /*
Todo:
 - Delete institution   
*/
        private readonly IPharmacyService _pharmacyService;
        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacies([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, pharmacies) = await _pharmacyService.GetPharmacies(filterOptions, page, size);
            if (status == 0 || pharmacies.Length > 0)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institutions = pharmacies });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacy(string id)
        {
            var (status, message, pharmacy) = await _pharmacyService.GetPharmacy(id);
            if (status == 0 || pharmacy == null)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institution = pharmacy });
        }

        [HttpPost]
        [Authorize(Roles = "HealthCenterAdmin, SuperAdmin, PharmacyAdmin")]
        public async Task<IActionResult> AddPharmacy([FromBody] PharmacyDTO pharmacy)
        {
            var (status, message, createdPharmacy) = await _pharmacyService.AddPharmacy(pharmacy);
            if (status == 0 || createdPharmacy == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = createdPharmacy });
        }

        [HttpPut("{id}")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UpdatePharmacyInfo([FromBody] UpdatePharmacyDTO pharmacyDTO, string id)
        {
            var (status, message, updatedPharmacy) = await _pharmacyService.UpdatePharmacy(pharmacyDTO, id);
            if (status == 0 || updatedPharmacy == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = updatedPharmacy });
        }

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> UpdatePharmacyVerification([FromQuery] bool verified, string id)
        {
            var (status, message, pharmacy) = await _pharmacyService.UpdateInstitutionVerification<Pharmacy>(id, verified);
            if (status == 0 || pharmacy == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = pharmacy });
        }

        [HttpPost("{id}/upload-license")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, pharmacy) = await _pharmacyService.UploadLicense<Pharmacy>(id, license);
            if (status == 0 || pharmacy == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = pharmacy });
        }
    }
}