using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class LaboratoryService : InstitutionService<Laboratory>, ILaboratoryService
    {
        public LaboratoryService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<(int, string?, LaboratoryDTO[])> GetLaboratories(FilterDefinition<Laboratory> filterDefinition, int page, int size)
        {
            return await GetInstitutions<LaboratoryDTO>(filterDefinition, page, size);
        }

        public async Task<(int, string?, Laboratory?)> GetLaboratory(string id)
        {
            return await GetInstitutionById<Laboratory>(id);
        }

        public async Task<(int, string, LaboratoryDTO?)> AddLaboratory(Laboratory laboratory)
        {
            return await AddInstitution<LaboratoryDTO>(laboratory);
        }

        public async Task<(int, string, LaboratoryDTO?)> UpdateLaboratory(LaboratoryDTO laboratoryDTO, string laboratoryId)
        {
            return await UpdateInstitution<LaboratoryDTO, LaboratoryDTO>(laboratoryDTO, laboratoryId);
        }
    }
}