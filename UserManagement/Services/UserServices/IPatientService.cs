using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;

namespace UserManagement.Services.UserServices
{
    public interface IPatientService : IUserService
    {
        Task<(int, string?, UsagePatientDTO?)> GetPatientById(string patientId);
        Task<(int, string?, UsagePatientDTO?)> GetPatientByEmail(string patientEmail);
        Task<(int, string, UsagePatientDTO?)> AddPatient(Patient patient);
        Task<(int, string, UsagePatientDTO?)> UpdatePatient(UpdatePatientDTO patientDTO, string id);
    }
}