using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class LaboratoryService : InstitutionService<Laboratory>, ILaboratoryService
    {
        private readonly IHealthCenterService _healthCenterService;
        private readonly IDoctorService _doctorService;
        private readonly IMongoCollection<TestRequest> _testRequestCollection;

        public LaboratoryService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IHealthCenterService healthCenterService, IAdminService adminService, IDoctorService doctorService) : base(options, fileService, mapper, adminService)
        {
            _healthCenterService = healthCenterService;
            _doctorService = doctorService;
            _testRequestCollection = GetCollection<TestRequest>("TestRequests");
        }

        public async Task<SResponseDTO<LaboratoryDTO[]>> GetLaboratories(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<LaboratoryDTO>(filterOption!, page, size);
        }

        public async Task<SResponseDTO<Laboratory>> GetLaboratory(string id)
        {
            return await GetInstitutionById<Laboratory>(id);
        }

        public async Task<SResponseDTO<TestRequestDTO>> GetLabTestRequest(string labTestId)
        {
            try
            {
                var testRequest = await _testRequestCollection.Find(tr => tr.Id == labTestId).FirstOrDefaultAsync();
                if (testRequest == null)
                    return new() { StatusCode = 404, Errors = new[] { "Test request not found" } };

                var testRequestDTO = _mapper.Map<TestRequestDTO>(testRequest);

                var laboratoryTask = Task.Run(() => GetLaboratory(testRequest.LaboratoryId));
                var doctorTask = Task.Run(() => _doctorService.GetDoctorById(testRequest.RequestorId));
                var laboratorianTask = Task.Run(() => _adminService.GetAdminById(testRequest.HandlerId));

                await Task.WhenAll(laboratoryTask, doctorTask, laboratorianTask);

                testRequestDTO.LaboratoryName = laboratoryTask.Result.Data?.Name ?? string.Empty;
                testRequestDTO.LaboratorianName = laboratorianTask.Result.Data?.Fullname ?? string.Empty;
                testRequestDTO.DoctorName = doctorTask.Result.Data?.Fullname ?? string.Empty;

                return new() { StatusCode = 200, Message = "Found test request", Data = testRequestDTO, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }



        public async Task<SResponseDTO<TestRequestDTO[]>> GetLabTestRequests(string laboratoryId)
        {
            try
            {
                var response = await GetLaboratory(laboratoryId);
                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                var labTestRequests = response.Data!.TestRequestIds;

                var filterBuilder = Builders<TestRequest>.Filter;
                var filter = filterBuilder.Eq(tr => tr.LaboratoryId, laboratoryId) & filterBuilder.Eq(tr => tr.Status, RequestStatus.Pending);
                var results = (await _testRequestCollection.Find(filter).ToListAsync()).ToArray();
                var testRequests = Array.Empty<TestRequestDTO>();


                var tasks = results.Select(async testRequest =>
                    {
                        var response = await GetLabTestRequest(testRequest.Id ?? string.Empty);
                        if (response.Success)
                            _ = testRequests.Append(response.Data!);
                    });

                await Task.WhenAll(tasks);

                return new() { StatusCode = 200, Data = testRequests, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }

        }

        public async Task<SResponseDTO<TestRequestDTO[]>> RequestForLabTest(CreateTestRequestDTO labTestRequest, string laboratoryId)
        {
            try
            {
                var response = await GetLaboratory(laboratoryId);

                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                //Add test request Id to the laboratory
                var laboratory = response.Data!;
                var testRequest = _mapper.Map<TestRequest>(labTestRequest);
                testRequest.LaboratoryId = laboratoryId;

                await _testRequestCollection.InsertOneAsync(testRequest);
                _ = laboratory.TestRequestIds.Append(testRequest.Id);

                var labUpdateResponse = await UpdateInstitution<Laboratory, LaboratoryDTO>(laboratory, laboratory.Id!);

                if (!labUpdateResponse.Success)
                    return new() { StatusCode = labUpdateResponse.StatusCode, Errors = labUpdateResponse.Errors };

                var createdTestRequest = _mapper.Map<TestRequest>(labTestRequest);

                return new() { StatusCode = 201, Message = "Test request created successfully", Data = null, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<LaboratoryDTO>> AddLaboratory(LaboratoryDTO laboratory, string adminId)
        {
            HealthCenterDTO? healthCenter = await HealthCenterExists(laboratory.HealthCenterName);
            string healthCenterId = healthCenter?.Id ?? string.Empty;

            if (laboratory.HealthCenterName == string.Empty || healthCenterId == string.Empty)
                return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };

            Laboratory _laboratory = _mapper.Map<Laboratory>(laboratory);
            _laboratory.Type = InstitutionType.Laboratory;
            _laboratory.AssociatedHealthCenterId = healthCenterId;
            return await AddInstitution<LaboratoryDTO>(_laboratory, adminId);
        }

        public async Task<SResponseDTO<LaboratoryDTO>> UpdateLaboratory(UpdateLaboratoryDTO laboratoryDTO, string laboratoryId)
        {
            HealthCenterDTO? healthCenter;
            string healthCenterId = string.Empty;

            if (laboratoryDTO.HealthCenterName != string.Empty)
            {
                healthCenter = await HealthCenterExists(laboratoryDTO.HealthCenterName);
                healthCenterId = healthCenter?.Id ?? string.Empty;

                if (healthCenterId == string.Empty)
                    return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };
            }

            var response = await UpdateInstitution<UpdateLaboratoryDTO, LaboratoryDTO>(laboratoryDTO, laboratoryId, healthCenterId);
            if (response.Success)
                response.Data!.HealthCenterName = laboratoryDTO.HealthCenterName;

            return new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Success = response.Success, Errors = response.Errors };
        }

        private async Task<HealthCenterDTO?> HealthCenterExists(string healthCenterName)
        {
            //Check if the health center exists

            var response = await _healthCenterService.GetHealthCenterByName(healthCenterName);
            return _mapper.Map<HealthCenterDTO>(response.Data);
        }

        public async Task<SResponseDTO<LaboratoryDTO>> UpdateLabTest(LabTest labTest, string laboratoryId)
        {
            var response = await GetLaboratory(laboratoryId);
            if (!response.Success)
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };

            var filter = Builders<Laboratory>.Filter.Eq(l => l.Id, laboratoryId);
            var update = Builders<Laboratory>.Update.Push(l => l.AvailableTests, labTest);
            var updateResult = await _collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return new SResponseDTO<LaboratoryDTO>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Errors = new[] { "No laboratory found with the provided ID." }
                };
            }
            else
            {
                return new SResponseDTO<LaboratoryDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lab test added successfully",
                    Success = true
                };
            }
        }

        public async Task<SResponseDTO<LaboratoryDTO>> DeleteLabTest(string labTestId, string laboratoryId)
        {
            var response = await GetLaboratory(laboratoryId);
            if (!response.Success)
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };

            var filter = Builders<Laboratory>.Filter.Eq(l => l.Id, laboratoryId);
            var update = Builders<Laboratory>.Update.PullFilter(l => l.AvailableTests, lt => lt.Id == labTestId);
            var updateResult = await _collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return new SResponseDTO<LaboratoryDTO>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Errors = new[] { "No laboratory found with the provided ID or the lab test was not found in the available tests." }
                };
            }
            else
            {
                return new SResponseDTO<LaboratoryDTO>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lab test deleted successfully",
                    Success = true
                };
            }

        }
    }
}