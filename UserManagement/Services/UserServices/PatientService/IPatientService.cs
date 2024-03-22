using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;

namespace UserManagement.Services.UserServices
{
    public interface IPatientService : IUserService
    {
        Task<SResponseDTO<UsagePatientDTO>> GetPatientById(string patientId);
        Task<SResponseDTO<UsagePatientDTO>> GetPatientByEmail(string patientEmail);
        Task<SResponseDTO<Patient>> AddPatient(Patient patient);
        Task<SResponseDTO<UsagePatientDTO>> UpdatePatient(UpdatePatientDTO patientDTO, string id);
    }
}