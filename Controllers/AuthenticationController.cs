using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services;

namespace UserAuthentication.Controllers
{
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
                var (status, message, user) = await _authService.Login(model);

                if (status == 0 || user == null)
                    return BadRequest(new { error = message });

                return Ok(new { token = message, message = "Login successful", user });
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