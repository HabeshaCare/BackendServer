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