using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UserDTO : IUserDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";
        public char Gender { get; set; }

        public virtual void MapFromUser(User user)
        {
            Email = user.Email;
        }
    }
}