using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.UserServices;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
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