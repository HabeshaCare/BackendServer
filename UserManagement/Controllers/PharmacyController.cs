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
            var response = await _pharmacyService.GetPharmacies(filterOptions, page, size);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacy(string id)
        {
            var response = await _pharmacyService.GetPharmacy(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [Authorize(Roles = "PharmacyAdmin, HealthCenterAdmin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddPharmacy([FromBody] PharmacyDTO pharmacy)
        {
            var response = await _pharmacyService.AddPharmacy(pharmacy);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UpdatePharmacyInfo([FromBody] UpdatePharmacyDTO pharmacyDTO, string id)
        {
            var response = await _pharmacyService.UpdatePharmacy(pharmacyDTO, id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}/verify")]
        [Authorize(Roles = "SuperAdmin, HealthCenterAdmin")]
        public async Task<IActionResult> UpdatePharmacyVerification([FromQuery] bool verified, string id)
        {
            var response = await _pharmacyService.UpdateInstitutionVerification<Pharmacy>(id, verified);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPost("{id}/upload-license")]
        [AuthorizeInstitutionAccess]
        public async Task<IActionResult> UploadLicense(string id, [FromForm] IFormFile license)
        {
            var response = await _pharmacyService.UploadLicense<Pharmacy>(id, license);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
    }
}