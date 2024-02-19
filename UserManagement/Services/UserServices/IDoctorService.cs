using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IDoctorService
    {
        Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(string doctorId);
        Task<(int, string?, UsageDoctorDTO[])> GetDoctors(DoctorFilterDTO filterOptions, int page, int size);
        Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId);
        Task<(int, string, UsageDoctorDTO?)> UpdateDoctor(UpdateDoctorDTO doctorDTO, string id);
        Task<(int, string, UsageDoctorDTO?)> UploadLiscense(IFormFile licenseInformation, string doctorId);



    }
}