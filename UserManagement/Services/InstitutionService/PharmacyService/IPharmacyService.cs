using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService
{
    public interface IPharmacyService : IInstitutionService
    {
        Task<SResponseDTO<List<PharmacyDTO>>> GetPharmacies(FilterDTO? filterOption, int page, int size);
        Task<SResponseDTO<Pharmacy>> GetPharmacy(string id);
        Task<SResponseDTO<PharmacyDTO>> AddPharmacy(PharmacyDTO pharmacyDTO, string adminId);
        Task<SResponseDTO<PharmacyDTO>> UpdatePharmacy(UpdatePharmacyDTO pharmacyDTO, string pharmacyId);
    }
}