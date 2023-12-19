using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace Backend.Services.User
{
    public interface IUserService
    {
        Task<(int, string, UsageUserDTO?)> Update(UpdateDTO model);
    }
}