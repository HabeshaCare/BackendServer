using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Services;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController>  _logger;
        public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var (status, message) = await _authService.Login(model);
                
                if (status == 0)
                    return BadRequest(message);
                return Ok(message);
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest("Invalid Payload");
                var (status, message) = await _authService.Registration(model);
                
                if(status == 0)
                    return BadRequest(message);
                
                return CreatedAtAction(nameof(Register), model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}