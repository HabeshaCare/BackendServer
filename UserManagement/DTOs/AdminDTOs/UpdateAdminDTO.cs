using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.DTOs.AdminDTOs
{
    public class UpdateAdminDTO : UpdateUserDTO
    {
        /* 
        Currently, this class is only needed to keep because 
        administrators are kept in a separate collection and 
        a different type is need to handle it
        */
    }
}