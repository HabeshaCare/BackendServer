using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
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
        public async Task<IActionResult> AddLaboratory([FromBody] PharmacyDTO pharmacy)
        {
            var (status, message, createdPharmacy) = await _pharmacyService.AddPharmacy(pharmacy);
            if (status == 0 || createdPharmacy == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = createdPharmacy });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLaboratoryInfo([FromBody] PharmacyDTO pharmacyDTO, string id)
        {
            var (status, message, updatedPharmacy) = await _pharmacyService.UpdatePharmacy(pharmacyDTO, id);
            if (status == 0 || updatedPharmacy == null)
                return BadRequest(new { errors = message });

            return Ok(new { success = true, message, institution = updatedPharmacy });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacyVerification([FromQuery] bool verified, string id)
        {
            var (status, message, pharmacy) = await _pharmacyService.UpdateInstitutionVerification<Pharmacy>(id, verified);
            if (status == 0 || pharmacy == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = pharmacy });
        }

        [HttpPost("{id}/upload-license")]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var (status, message, pharmacy) = await _pharmacyService.UploadLicense<Pharmacy>(id, license);
            if (status == 0 || pharmacy == null)
                return BadRequest(new { errors = message });
            return Ok(new { success = true, message, institution = pharmacy });
        }
    }
}