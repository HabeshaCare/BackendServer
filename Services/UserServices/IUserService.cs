using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string, UsageUserDTO?)> Update(UpdateDTO model);
    }
}