using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.UserServices;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Authorize(Roles= "Normal, Doctor, Admin")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService,IScheduleService scheduleService, ILogger<UserController> logger)
        {
            _userService = userService;
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [HttpGet("schedule/")]
        public async Task<IActionResult> GetSchedules([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            role = role.IsNullOrEmpty() ? "" : userId;
            bool scheduler = role == UserRole.Normal.ToString();

            var (status, message, schedule) = await _scheduleService.GetSchedules(userId!, scheduler, page, size);
            
            if(status == 0 || schedule == null)
                return NotFound(new{message});
            
            return Ok(new{message, schedule});
        }

        [HttpGet("schedule/{id}")]
        public async Task<IActionResult> GetSchedule(string id)
        {
            var (status, message, schedule) = await _scheduleService.GetSchedule(id);
            
            if(status == 0 || schedule == null)
                return NotFound(new{message});
            
            return Ok(new{message, schedule});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UploadImage([FromForm]IFormFile image,string id)
        {
            UpdateDTO model = new();
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                
                model.Id= id;
                model.Image = image;
                var (status, message, user) = await _userService.Update(model);

                if(status == 0 || user == null)
                    return BadRequest(new{error=message});
                
                return Ok(new{message = "User updated successfully", user});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }
    }
}