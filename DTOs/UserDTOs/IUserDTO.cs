using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public interface IUserDTO
    {
        void MapFromUser(User user);
    }
}