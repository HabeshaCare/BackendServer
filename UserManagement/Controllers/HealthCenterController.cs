using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.PatientDTOs;
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
        private static readonly TimeSpan sharedPatientExpires = TimeSpan.FromDays(2); // Duration of patient sharing
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

        [HttpGet("{id}/patients")]
        [Authorize(Roles = "HealthCenterAdmin, Doctor")]
        public async Task<IActionResult> GetSharedPatients(string id)
        {
            var response = await _healthCenterService.GetSharedPatients(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPost("{id}/patients/{patientId}")]
        [Authorize(Roles = "Reception")]
        public async Task<IActionResult> SharePatient(string id, string patientId)
        {
            var response = await _healthCenterService.SharePatient(id, patientId, sharedPatientExpires);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPost("{id}/patients/{patientId}/refer")]
        [Authorize(Roles = "Doctor")]
        [AuthorizePatientAccess]
        public async Task<IActionResult> ReferPatient(string id, string patientId, [FromBody] ReferralDTO referralDTO)
        {
            //#TODO: Add medical report and a separate referring service here or in the healthCenter service
            referralDTO.PatientId = patientId;
            var response = await _healthCenterService.ReferPatient(referralDTO, sharedPatientExpires);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
    }
}