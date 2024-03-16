using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService
{
    public interface ILaboratoryService : IInstitutionService
    {
        Task<(int, string?, LaboratoryDTO[])> GetLaboratories(FilterDTO? filterOption, int page, int size);
        Task<(int, string?, Laboratory?)> GetLaboratory(string id);
        Task<(int, string, LaboratoryDTO?)> AddLaboratory(LaboratoryDTO laboratory);
        Task<(int, string, LaboratoryDTO?)> UpdateLaboratory(UpdateLaboratoryDTO laboratoryDTO, string laboratoryId);
    }
}