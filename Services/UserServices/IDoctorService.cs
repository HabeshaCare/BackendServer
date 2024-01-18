using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Models.DTOs.OptionsDTO;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IDoctorService
    {
        Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(string doctorId);
        Task<(int, string?, UsageDoctorDTO[])> GetDoctors(int page, int size);
        Task<(int, string?, UsageDoctorDTO[])> GetDoctors(int page, int size, DoctorFilterDTO filterOptions);
        Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId);
        Task<(int, string, UsageDoctorDTO?)> Update(UpdateDoctorDTO doctorDTO, String id);

    }
}