using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication operations.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var (status, tokenOrMessage, user) = await _authService.Login(model);

                //Checking if the method executed successfully
                if (status == 0 || user == null)
                    return BadRequest(new { error = tokenOrMessage });

                Response.Cookies.Append("token", tokenOrMessage, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.None,
                    MaxAge = TimeSpan.FromDays(1),

                });

                // Set authentication cookies.
                Response.Cookies.Append("userId", user.Id, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.None,
                    MaxAge = TimeSpan.FromDays(1),

                });

                return Ok(new { token = tokenOrMessage, message = "Login successful", user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            try
            {
                var request = Request;
                if (!ModelState.IsValid)
                    return BadRequest("Invalid Payload");
                var (status, message, user) = await _authService.Registration(model);

                if (status == 0)
                    return BadRequest(new { errors = message });

                return CreatedAtAction(nameof(Register), user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }


    }
}