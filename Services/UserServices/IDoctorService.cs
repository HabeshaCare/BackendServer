using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IDoctorService
    {
        Task<(int, string, UsageDoctorDTO?)> Update(UpdateDoctorDTO doctorDTO, String id);
    }
}