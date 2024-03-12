using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IDoctorService : IUserService
    {
        Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(string doctorId);
        Task<(int, string?, UsageDoctorDTO?)> GetDoctorByEmail(string doctorEmail);
        Task<(int, string?, UsageDoctorDTO[])> GetDoctors(FilterDTO filterOptions, int page, int size);
        Task<(int, string, Doctor?)> AddDoctor(Doctor user);
        Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId);
        Task<(int, string, Doctor?)> UpdateDoctor(UpdateDoctorDTO doctorDTO, string id);
        Task<(int, string, UsageDoctorDTO?)> UploadLicense(IFormFile licenseInformation, string doctorId);

    }
}