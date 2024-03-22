using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UserManagement.DTOs.UserDTOs;
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
                var response = await _authService.Login(model);

                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
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
                var response = await _authService.Registration(model);
                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var response = await _authService.VerifyEmail(token);

            return new ObjectResult(response);

        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            var response = await _authService.ForgotPassword(email);

            return new ObjectResult(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDTO request)
        {
            var response = await _authService.ResetPassword(request);

            return new ObjectResult(response);
        }

    }
}