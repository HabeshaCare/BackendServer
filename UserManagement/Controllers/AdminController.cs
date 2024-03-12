using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Services.UserServices;

namespace UserManagement.Controllers
{
    [ApiController]
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
            var (status, message, admin) = await _adminService.GetAdminById(id);
            if (status == 0 || admin == null)
            {
                return NotFound(new { error = message });
            }
            return Ok(new { success = true, user = admin });
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] UpdateAdminDTO adminDTO)
        {
            var (status, message, admin) = await _adminService.UpdateAdmin(adminDTO, id);

            if (status == 0 || admin == null)
                return BadRequest(new { error = message });

            return Ok(new { message, user = admin });
        }

        [HttpPost("{id}/profile/upload-picture")]
        public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        {
            try
            {
                var (status, message, user) = await _adminService.UploadProfilePic<Administrator>(id, image);

                if (status == 0 || user == null)
                    return BadRequest(new { error = message });
                return Ok(new { message, user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }
    }
}