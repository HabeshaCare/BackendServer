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
        Task<(int, string?, UsageDoctorDTO[])> GetDoctors(DoctorFilterDTO filterOptions, int page, int size);
        Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId);
        Task<(int, string, UsageDoctorDTO?)> UpdateDoctor(UpdateDoctorDTO doctorDTO, String id);
        Task<(int, string, UsageDoctorDTO?)> UploadLiscense(IFormFile licenseInformation, string doctorId);



    }
}