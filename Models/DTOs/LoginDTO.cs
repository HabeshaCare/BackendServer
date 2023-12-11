using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs
{
    public class LoginDTO
    {
    public string Name { get; set; } = "";
        public string Password {get; set;} = "";
    }
}