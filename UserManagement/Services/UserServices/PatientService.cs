using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class PatientService : UserService<Patient>, IPatientService
    {
        public PatientService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<(int, string, UsagePatientDTO?)> AddPatient(Patient user)
        {
            return await AddUser<UsagePatientDTO>(user);
        }

        public async Task<(int, string?, UsagePatientDTO?)> GetPatientById(string patientId)
        {
            return await GetUserById<UsagePatientDTO>(patientId);
        }

        public async Task<(int, string, UsagePatientDTO?)> UpdatePatient(UpdatePatientDTO patientDTO, string id)
        {
            return await UpdateUser<UpdatePatientDTO, UsagePatientDTO>(patientDTO, id);
        }
    }
}