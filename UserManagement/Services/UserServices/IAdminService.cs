using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Services.UserServices
{
    public interface IAdminService
    {
        Task<(int, string?, UsagePatientDTO?)> GetPatientById(string patientId);
    }
}