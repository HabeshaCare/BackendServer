using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}