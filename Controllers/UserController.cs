using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserAuthentication.DTOs.ScheduleDTOs;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.UserServices;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Authorize(Roles= "Normal, Doctor, Admin")]
    [Route("api/user")]
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
            role = role.IsNullOrEmpty() ? "" : role;
            bool scheduler = role == UserRole.Normal.ToString();

            var (status, message, schedule) = await _scheduleService.GetSchedules(userId!, scheduler, page, size);
            
            if(status == 0 || schedule == null)
                return NotFound(new{errors=message});
            
            return Ok(new{message, schedule});
        }

        [HttpGet("schedule/{id}")]
        public async Task<IActionResult> GetSchedule(string id)
        {
            var (status, message, schedule) = await _scheduleService.GetScheduleById(id);
            
            if(status == 0 || schedule == null)
                return NotFound(new{errors=message});
            
            return Ok(new{message, schedule});
        }

        [HttpPost("schedule/")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO schedule)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            if(role == UserRole.Doctor.ToString())
                return Forbid("Doctor can't create schedule");

            var (status, message, createdSchedule) = await _scheduleService.CreateSchedule(schedule, userId!);
            
            if(status == 0 || createdSchedule == null)
                return StatusCode(500, new{errors=message});
            
            return Ok(new{message, schedule=createdSchedule});
        }

        [HttpPut("schedule/{id}")]
        public async Task<IActionResult> UpdateSchedule([FromBody] DateTime dateTime, string scheduleId)
        {
            var (status, message, updatedSchedule) = await _scheduleService.UpdateSchedule(dateTime, scheduleId);
            
            if(status == 0 || updatedSchedule == null)
                return StatusCode(500, new{errors=message});
            return Ok(new{message, schedule=updatedSchedule});
        }
        
        [HttpPut("schedule/{id}/status")]
        public async Task<IActionResult> UpdateScheduleStatus(string scheduleId, [FromBody] bool scheduleStatus)
        {
            string? role = HttpContext.Items["Role"]?.ToString();
            if(role == UserRole.Doctor.ToString())
                return Forbid("Only doctor can accept invitation");

            var (status, message, updatedSchedule) = await _scheduleService.UpdateScheduleStatus(scheduleId, scheduleStatus);
            
            if(status == 0 || updatedSchedule == null)
                return StatusCode(500, new{errors=message});
            return Ok(new{message, schedule=updatedSchedule});
        }


        [HttpDelete("schedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            var (status, message) = await _scheduleService.DeleteSchedule(scheduleId);
            
            if(status == 0)
                return StatusCode(500, new{errors=message});
            return Ok(new{message});
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