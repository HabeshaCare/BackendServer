using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.InstitutionService.HealthCenterService;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCenterController : ControllerBase
    {
        /*
        Todo: 
         - Get institution based on filter conditions from query. 
            1. Using id
            2. Using admin's id
            3. Filter verified institutions only
            4. Filter Unverified institutions only
            5. Pagination should be implemented by default
         - Create institution
         - Update institution
         - Delete institution   
        */

        private readonly IHealthCenterService _healthCenterService;
        public HealthCenterController(IHealthCenterService healthCenterService)
        {
            _healthCenterService = healthCenterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthCenters([FromQuery] FilterDTO? filterOptions = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (status, message, healthCenter) = await _healthCenterService.GetHealthCenters(filterOptions, page, size);
            if (status == 0 || healthCenter == null)
                NotFound(new { errors = message });
            return Ok(new { success = true, message, institution = healthCenter });
        }
    }
}