using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService
{
    public interface IPharmacyService : IInstitutionService
    {
        Task<(int, string?, PharmacyDTO[])> GetLaboratories(FilterDTO? filterOption, int page, int size);
        Task<(int, string?, Pharmacy?)> GetPharmacy(string id);
        Task<(int, string, PharmacyDTO?)> AddPharmacy(Pharmacy pharmacy);
        Task<(int, string, PharmacyDTO?)> UpdatePharmacy(PharmacyDTO pharmacyDTO, string pharmacyId);
    }
}