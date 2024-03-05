using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;

namespace UserManagement.Services.UserServices
{
    public interface IPatientService
    {
        Task<(int, string?, UsagePatientDTO?)> GetPatientById(string patientId);
        Task<(int, string?, UsagePatientDTO?)> GetPatientByEmail(string email);
        Task<(int, string, UsagePatientDTO?)> AddPatient(Patient user);
        Task<(int, string, UsagePatientDTO?)> UpdatePatient(UpdatePatientDTO patientDTO, string id);
    }
}