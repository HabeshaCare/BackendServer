using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles="Admin")]

    public class UserListController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userlist = await Task.FromResult(new string[] {"YabD", "Miki", "Nati", "YabN"});
            return Ok(userlist);
        }
    }
}