using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models
{
    public class UserDTO
    {
        public string Name { get; set; } = "";
        public int Age { get; set; } = 0;
        public string Role { get; set; } = "User";
    }
}