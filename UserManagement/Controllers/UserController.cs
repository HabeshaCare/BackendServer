using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManagement.DTOs.ScheduleDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.ChatServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Controllers
{
    //# TODO: This will be refactored as only methods that are not role specific. The user specific methods will be moved to their own respective controllers.

    /// <summary>
    /// Controller responsible for handling user-related operations. user can be any one in our system.
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Normal, Doctor, Admin")]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IChatAIService _chatAIService;
        private readonly IScheduleService _scheduleService;
        public UserController(IScheduleService scheduleService, IChatAIService chatAIService, ILogger<UserController> logger)
        {
            _scheduleService = scheduleService;
            _chatAIService = chatAIService;
        }

        /// <summary>
        /// Get a list of schedules for the user or doctor.
        /// </summary>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="size">Number of items per page (default is 10).</param>
        /// <returns>ActionResult containing the list of schedules.</returns>
        [HttpGet("schedule/")]
        public async Task<IActionResult> GetSchedules([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            role = role.IsNullOrEmpty() ? "" : role;
            bool scheduler = role != UserRole.Doctor.ToString();

            var (status, message, schedule) = await _scheduleService.GetSchedules(userId!, scheduler, page, size);

            if (status == 0 || schedule == null)
                return NotFound(new { errors = message });

            return Ok(new { message, schedule });
        }

        [HttpGet("schedule/{id}")]
        public async Task<IActionResult> GetSchedule(string id)
        {
            var (status, message, schedule) = await _scheduleService.GetScheduleById(id);

            if (status == 0 || schedule == null)
                return NotFound(new { errors = message });

            return Ok(new { message, schedule });
        }

        [HttpPost("schedule/")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO schedule)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            if (role == UserRole.Doctor.ToString())
                return Forbid("Doctor can't create schedule");

            var (status, message, createdSchedule) = await _scheduleService.CreateSchedule(schedule, userId!);

            if (status == 0 || createdSchedule == null)
                return StatusCode(500, new { errors = message });

            return Ok(new { message, schedule = createdSchedule });
        }

        /// <summary>
        /// Update the schedule time for a specific schedule.
        /// </summary>
        [HttpPut("schedule/{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule([FromBody] DateTime dateTime, string scheduleId)
        {
            bool scheduler = HttpContext.Items["Role"]?.ToString() != UserRole.Doctor.ToString();
            var (status, message, updatedSchedule) = await _scheduleService.UpdateSchedule(dateTime, scheduleId, scheduler);

            if (status == 0 || updatedSchedule == null)
                return StatusCode(500, new { errors = message });
            return Ok(new { message, schedule = updatedSchedule });
        }

        /// <summary>
        /// Update the status of a specific schedule (confirmed or not).
        /// </summary>

        [HttpPut("schedule/{scheduleId}/status")]
        public async Task<IActionResult> UpdateScheduleStatus(string scheduleId, [FromBody] bool scheduleStatus)
        {
            string? role = HttpContext.Items["Role"]?.ToString();
            if (role != UserRole.Doctor.ToString())
                return Forbid();

            var (status, message, updatedSchedule) = await _scheduleService.UpdateScheduleStatus(scheduleId, scheduleStatus);

            if (status == 0 || updatedSchedule == null)
                return StatusCode(500, new { errors = message });
            return Ok(new { message, schedule = updatedSchedule });
        }


        [HttpDelete("schedule/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            var (status, message) = await _scheduleService.DeleteSchedule(scheduleId);

            if (status == 0)
                return StatusCode(500, new { errors = message });
            return Ok(new { message, success = true });
        }

        // [HttpPost("{id}/picture/")]
        // public async Task<IActionResult> UploadProfilePicture(string id, [FromForm] IFormFile? image)
        // {
        //     try
        //     {
        //         var (status, message, user) = await _userService.UploadProfile(id, image);

        //         if (status == 0 || user == null)
        //             return BadRequest(new { error = message });

        //         return Ok(new { message, user });

        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex.Message);
        //         return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
        //     }
        // }

        [HttpGet("{id}/chat/")]
        public async Task<IActionResult> GetUserMessages(string id)
        {
            var (status, message, messages) = await _chatAIService.GetMessages(id);
            if (status == 0)
                return BadRequest(new { error = message });

            return Ok(new { successMessage = message, messages });
        }

        [HttpPost("{id}/chat/")]
        public async Task<IActionResult> AskAI([FromBody] string message, string id)
        {
            var (status, statusMessage, response) = await _chatAIService.AskAI(id, message);
            if (status == 0 || response == null)
                return NotFound(new { error = "AI server not found" });
            return Ok(new { response, message = statusMessage });
        }
    }
}