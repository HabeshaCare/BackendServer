using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
    }
}