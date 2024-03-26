using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Services.UserServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var response = await _adminService.GetAdminById(id);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}/profile")]
        [AuthorizeAccess]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] UpdateAdminDTO adminDTO)
        {
            var response = await _adminService.UpdateAdmin(adminDTO, id);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPut("{id}/verify")]
        [Authorize(Roles = "HealthCenterAdmin")]
        public async Task<IActionResult> UpdateAdminVerification(string id, bool verified)
        {
            var response = await _adminService.UpdateVerification(verified, id);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        /// <summary>
        /// Add a pharmacyAdmin or laboratoryAdmin access to institutions associated with the health center. It accepts the institution id from the body.
        /// </summary>
        /// <param name="id">The id of the admin to grant access.</param>
        /// <param name="institutionId">The id of the laboratory or the pharmacy institution.</param>
        /// <returns>Nothing useful except for messages.</returns>
        [HttpPut("{id}/add-admin")]
        [Authorize(Roles = "HealthCenterAdmin")]
        public async Task<IActionResult> AddAdmin(string id, string institutionId)
        {
            var response = await _adminService.AddInstitutionAccess(id, institutionId);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }


        /// <summary>
        /// Remove a pharmacy or laboratory access to institutions associated with the health center.
        /// </summary>
        /// <param name="id">The id of the admin to revoke access.</param>
        /// <returns>Nothing useful except for messages.</returns>
        [HttpPut("{id}/remove-admin")]
        [Authorize(Roles = "HealthCenterAdmin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var response = await _adminService.RemoveInstitutionAccess(id);

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        [HttpPost("{id}/profile/upload-picture")]
        [AuthorizeAccess]
        public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        {
            try
            {
                var response = await _adminService.UploadProfilePic<UsageAdminDTO>(id, image);

                return new ObjectResult(response) { StatusCode = response.StatusCode };

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }
    }
}