using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Attributes;
using UserManagement.DTOs.ScheduleDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.ChatServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Controllers
{
    /// <summary>
    /// Controller responsible for handling user-related operations. user can be any one in our system.
    /// </summary>
    [ApiController]
    [Authorize]
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
        [AuthorizeAccess]
        public async Task<IActionResult> GetSchedules([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            role = role.IsNullOrEmpty() ? "" : role;
            bool scheduler = role != UserRole.Doctor.ToString();

            var response = await _scheduleService.GetSchedules(userId!, scheduler, page, size);

            return new ObjectResult(response);

        }

        [HttpGet("schedule/{id}")]
        [AuthorizeAccess]
        public async Task<IActionResult> GetSchedule(string id)
        {
            var response = await _scheduleService.GetScheduleById(id);

            return new ObjectResult(response);

        }

        [HttpPost("schedule/")]
        [AuthorizeAccess]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO schedule)
        {
            string? userId = HttpContext.Items["UserId"]?.ToString();
            string? role = HttpContext.Items["Role"]?.ToString();

            userId = userId.IsNullOrEmpty() ? "" : userId;
            if (role == UserRole.Doctor.ToString())
                return Forbid("Doctor can't create schedule");

            var response = await _scheduleService.CreateSchedule(schedule, userId!);

            return new ObjectResult(response);

        }

        /// <summary>
        /// Update the schedule time for a specific schedule.
        /// </summary>
        [HttpPut("schedule/{scheduleId}")]
        [AuthorizeAccess]
        public async Task<IActionResult> UpdateSchedule([FromBody] DateTime dateTime, string scheduleId)
        {
            bool scheduler = HttpContext.Items["Role"]?.ToString() != UserRole.Doctor.ToString();
            var response = await _scheduleService.UpdateSchedule(dateTime, scheduleId, scheduler);

            return new ObjectResult(response);

        }

        /// <summary>
        /// Update the status of a specific schedule (confirmed or not).
        /// </summary>

        [HttpPut("schedule/{scheduleId}/status")]
        [AuthorizeAccess]
        public async Task<IActionResult> UpdateScheduleStatus(string scheduleId, [FromBody] bool scheduleStatus)
        {
            string? role = HttpContext.Items["Role"]?.ToString();
            if (role != UserRole.Doctor.ToString())
                return Forbid();

            var response = await _scheduleService.UpdateScheduleStatus(scheduleId, scheduleStatus);

            return new ObjectResult(response);
        }


        [HttpDelete("schedule/{scheduleId}")]
        [AuthorizeAccess]
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            var response = await _scheduleService.DeleteSchedule(scheduleId);

            return new ObjectResult(response);

        }
    }
}