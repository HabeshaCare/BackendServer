using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class PharmacyService : InstitutionService<Pharmacy>, IPharmacyService
    {
        public PharmacyService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<(int, string, PharmacyDTO?)> AddPharmacy(PharmacyDTO pharmacyDTO)
        {
            Pharmacy pharmacy = _mapper.Map<Pharmacy>(pharmacyDTO);
            return await AddInstitution<PharmacyDTO>(pharmacy);
        }

        public async Task<(int, string?, PharmacyDTO[])> GetPharmacies(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<PharmacyDTO>(filterOption!, page, size);

        }

        public async Task<(int, string?, Pharmacy?)> GetPharmacy(string id)
        {
            return await GetInstitutionById<Pharmacy>(id);
        }

        public async Task<(int, string, PharmacyDTO?)> UpdatePharmacy(PharmacyDTO pharmacyDTO, string pharmacyId)
        {
            return await UpdateInstitution<PharmacyDTO, PharmacyDTO>(pharmacyDTO, pharmacyId);

        }
    }
}