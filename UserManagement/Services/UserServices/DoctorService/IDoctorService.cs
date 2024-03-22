using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IDoctorService : IUserService
    {
        Task<SResponseDTO<UsageDoctorDTO>> GetDoctorById(string doctorId);
        Task<SResponseDTO<UsageDoctorDTO>> GetDoctorByEmail(string doctorEmail);
        Task<SResponseDTO<UsageDoctorDTO[]>> GetDoctors(FilterDTO filterOptions, int page, int size);
        Task<SResponseDTO<Doctor>> AddDoctor(Doctor user);
        Task<SResponseDTO<UsageDoctorDTO>> VerifyDoctor(string doctorId);
        Task<SResponseDTO<UsageDoctorDTO>> UpdateDoctor(UpdateDoctorDTO doctorDTO, string id);
        Task<SResponseDTO<UsageDoctorDTO>> UploadLicense(IFormFile licenseInformation, string doctorId);

    }
}