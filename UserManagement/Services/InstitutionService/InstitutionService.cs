using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class InstitutionService<T> : MongoDBService, IInstitutionService where T : Institution
    {

        protected readonly IMongoCollection<T> _collection;
        protected readonly IFileService _fileService;
        protected readonly IAdminService _adminService;
        protected readonly IMapper _mapper;
        public InstitutionService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IAdminService adminService) : base(options)
        {
            _collection = GetCollection<T>($"{typeof(T).Name}s");
            _fileService = fileService;
            _adminService = adminService;
            _mapper = mapper;
        }

        // USD refers to the Usage DTO of an institution
        protected async Task<SResponseDTO<USD[]>> GetInstitutions<USD>(FilterDTO filterOptions, int page, int size)
        {
            var filterBuilder = Builders<T>.Filter;
            var filterDefinition = filterBuilder.Empty;

            if (filterOptions.Verified.HasValue)
                filterDefinition &= filterBuilder.Gte("Verified", filterOptions.Verified);

            if (!string.IsNullOrEmpty(filterOptions.Freelancer))
                filterDefinition &= filterBuilder.Eq("AssociatedHealthCenterId", string.Empty);

            int skip = (page - 1) * size;
            try
            {
                var foundInstitutions = await _collection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundInstitutions.Count == 0)
                    return new() { StatusCode = StatusCodes.Status404NotFound, Errors = new[] { "No matching institutions found" } };

                USD[] institutions = _mapper.Map<USD[]>(foundInstitutions);
                return new() { StatusCode = StatusCodes.Status200OK, Message = $"Found {foundInstitutions.Count} matching institutions", Data = institutions, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        protected async Task<SResponseDTO<T>> GetInstitution(string id)
        {
            try
            {
                var result = await _collection.FindAsync(I => I.Id == id);
                T? institution = (await result.ToListAsync()).FirstOrDefault();
                return new() { StatusCode = StatusCodes.Status200OK, Message = "Institution found", Data = institution, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        // USD refers to the Usage DTO of a institution
        public async Task<SResponseDTO<USD>> GetInstitutionById<USD>(string id)
        {
            var response = await GetInstitution(id);
            if (response.Success)
            {
                T institution = response.Data!;
                USD? foundInstitution = _mapper.Map<USD>(institution);
                return new() { StatusCode = StatusCodes.Status200OK, Message = "Institution found", Data = foundInstitution, Success = true };
            }
            return new() { StatusCode = response.StatusCode, Errors = response.Errors };
        }

        protected async Task<SResponseDTO<USD>> GetInstitutionByName<USD>(string name)
        {
            try
            {
                var result = await _collection.FindAsync(I => I.Name == name);
                T? institution = (await result.ToListAsync()).FirstOrDefault();
                USD? foundInstitution = _mapper.Map<USD>(institution);

                return new() { StatusCode = StatusCodes.Status200OK, Message = "Institution found", Data = foundInstitution, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        // AD refers to the Registration DTO of an institution
        // USD refers to the Usage DTO of an institution
        protected async Task<SResponseDTO<USD>> AddInstitution<USD>(T institution, string adminId)
        {
            try
            {

                var response = await GetInstitutionByName<T>(institution.Name ?? "");

                if (response.Success)
                    return new() { StatusCode = StatusCodes.Status409Conflict, Errors = new[] { "Institution with this name already exists" } };

                await _collection.InsertOneAsync(institution);
                USD createdInstitution = _mapper.Map<USD>(institution);

                var accessResponse = await _adminService.AddInstitutionAccess(response.Data!.Id!, adminId);
                var failureMessage = accessResponse.Success ? string.Empty : "but couldn't grant access to admin";

                return new() { StatusCode = StatusCodes.Status201Created, Message = $"Institution created successfully {failureMessage}", Data = createdInstitution, Success = true, Errors = accessResponse.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        // UD refers to the Update DTO of a institution
        // USD refers to the Usage DTO of a institution
        protected async Task<SResponseDTO<USD>> UpdateInstitution<UD, USD>(UD institutionDTO, string institutionId, string healthCenterId = "")
        {
            try
            {
                var response = await GetInstitution(institutionId);
                if (response.Success)
                {
                    T institution = response.Data!;
                    _mapper.Map(institutionDTO, institution);

                    institution.Verified = false;

                    if (healthCenterId != "")
                        institution.AssociatedHealthCenterId = healthCenterId;

                    var filter = Builders<T>.Filter.And(
                        Builders<T>.Filter.Eq(u => u.Id, institutionId));

                    var options = new FindOneAndReplaceOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After // or ReturnDocument.Before
                    };

                    var result = await _collection.FindOneAndReplaceAsync(filter, institution, options);


                    USD updatedInstitutionDTO = _mapper.Map<USD>(result);

                    if (result == null)
                    {
                        return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { "Error updating institution" } };
                    }
                    return new() { StatusCode = StatusCodes.Status201Created, Message = "Institution updated successfully", Data = updatedInstitutionDTO, Success = true };
                }
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        //USD refers to the Usage DTO of an institution
        public async Task<SResponseDTO<USD>> UploadLicense<USD>(string institutionId, IFormFile? image)
        {
            var filter = Builders<T>.Filter.Eq(I => I.Id, institutionId);
            try
            {
                string? licensePath = null;
                if (image != null)
                {
                    var response = await _fileService.UploadFile(image, institutionId, "Licenses");
                    if (!response.Success)
                        return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                    licensePath = response.Data;
                }
                var getResponse = await GetInstitution(institutionId);

                if (!getResponse.Success)
                    return new() { StatusCode = getResponse.StatusCode, Errors = getResponse.Errors };
                T institution = getResponse.Data!;
                institution.LicensePath = licensePath ?? string.Empty;

                var options = new FindOneAndReplaceOptions<T>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var rawInstitution = await _collection.FindOneAndReplaceAsync(filter, institution, options);

                USD updatedInstitution = _mapper.Map<USD>(rawInstitution);
                return new() { StatusCode = StatusCodes.Status201Created, Message = "License information Uploaded Successfully", Data = updatedInstitution, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }

        //USD refers to the Usage DTO of an institution
        public async Task<SResponseDTO<USD>> UpdateInstitutionVerification<USD>(string institutionId, bool status)
        {
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq("_id", ObjectId.Parse(institutionId))
            );

            var update = Builders<T>.Update.Set(I => I.Verified, status);
            var options = new FindOneAndUpdateOptions<T>
            {
                ReturnDocument = ReturnDocument.After,
            };
            try
            {
                var result = await _collection.FindOneAndUpdateAsync(filter, update, options);
                if (result != null)
                    return new() { StatusCode = StatusCodes.Status201Created, Message = $"Institution Verification set to {status}", Data = _mapper.Map<USD>(result), Success = true };
                else
                    return new() { StatusCode = StatusCodes.Status404NotFound, Errors = new[] { "Institution not found" } };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new[] { ex.Message } };
            }
        }
    }

}