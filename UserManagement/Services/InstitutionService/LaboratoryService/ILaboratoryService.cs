using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService
{
    public interface ILaboratoryService : IInstitutionService
    {
        Task<SResponseDTO<LaboratoryDTO[]>> GetLaboratories(FilterDTO? filterOption, int page, int size);
        Task<SResponseDTO<Laboratory>> GetLaboratory(string id);
        Task<SResponseDTO<TestRequestDTO>> GetLabTestRequest(string labTestId);
        Task<SResponseDTO<TestRequestDTO[]>> GetLabTestRequests(string laboratoryId);
        Task<SResponseDTO<TestRequestDTO[]>> RequestForLabTest(CreateTestRequestDTO labTestRequest, string id);
        Task<SResponseDTO<LaboratoryDTO>> AddLaboratory(LaboratoryDTO laboratory, string adminId);
        Task<SResponseDTO<LaboratoryDTO>> UpdateLaboratory(UpdateLaboratoryDTO laboratoryDTO, string laboratoryId);
        Task<SResponseDTO<LaboratoryDTO>> UpdateLabTest(LabTest labTest, string laboratoryId);
        Task<SResponseDTO<LaboratoryDTO>> DeleteLabTest(string labTestId, string laboratoryId);
    }
}