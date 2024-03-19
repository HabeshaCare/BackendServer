using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class InstitutionService<T> : MongoDBService, IInstitutionService where T : Institution
    {

        protected readonly IMongoCollection<T> _collection;
        protected readonly IFileService _fileService;
        protected readonly IMapper _mapper;
        public InstitutionService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options)
        {
            _collection = GetCollection<T>($"{typeof(T).Name}s");
            _fileService = fileService;
            _mapper = mapper;
        }

        // USD refers to the Usage DTO of an institution
        protected async Task<(int, string?, USD[])> GetInstitutions<USD>(FilterDTO filterOptions, int page, int size)
        {
            var filterBuilder = Builders<T>.Filter;
            var filterDefinition = filterBuilder.Empty;

            if (filterOptions.Verified.HasValue)
                filterDefinition &= filterBuilder.Gte("Verified", filterOptions.Verified);

            int skip = (page - 1) * size;
            try
            {
                var foundInstitutions = await _collection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundInstitutions.Count == 0)
                    return (0, "No matching institutions found", Array.Empty<USD>());

                USD[] institutions = _mapper.Map<USD[]>(foundInstitutions);
                return (1, $"Found {foundInstitutions.Count} matching institutions", institutions);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, Array.Empty<USD>())!;
            }
        }

        protected async Task<(int, string?, T?)> GetInstitution(string id)
        {
            try
            {
                var result = await _collection.FindAsync(I => I.Id == id);
                T? institution = (await result.ToListAsync()).FirstOrDefault();
                return (1, null, institution);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        // USD refers to the Usage DTO of a institution
        public async Task<(int, string?, USD?)> GetInstitutionById<USD>(string id)
        {
            var (status, message, institution) = await GetInstitution(id);
            if (status == 1 && institution != null)
            {
                USD? foundInstitution = _mapper.Map<USD>(institution);
                return (status, message, foundInstitution);
            }

            return (status, message, default(USD));
        }

        protected async Task<(int, string?, USD?)> GetInstitutionByName<USD>(string name)
        {
            try
            {
                var result = await _collection.FindAsync(I => I.Name == name);
                T? institution = (await result.ToListAsync()).FirstOrDefault();
                USD? foundInstitution = _mapper.Map<USD>(institution);

                return (1, null, foundInstitution);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // AD refers to the Registration DTO of an institution
        // USD refers to the Usage DTO of an institution
        protected async Task<(int, string, USD?)> AddInstitution<USD>(T institution)
        {
            try
            {
                var (status, message, foundInstitution) = await GetInstitutionById<T>(institution.Id ?? "");

                if (status == 1 && foundInstitution != null)
                    return (0, "Institution already exists", default(USD));

                await _collection.InsertOneAsync(institution);
                USD createdInstitution = _mapper.Map<USD>(institution);

                return (1, "Institution created successfully", createdInstitution);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // UD refers to the Update DTO of a institution
        // USD refers to the Usage DTO of a institution
        protected async Task<(int, string, USD?)> UpdateInstitution<UD, USD>(UD institutionDTO, string institutionId, string healthCenterId = "")
        {
            try
            {
                var (status, message, institution) = await GetInstitution(institutionId);
                if (status == 1 && institution != null)
                {
                    _mapper.Map(institutionDTO, institution);

                    institution.Verified = false;

                    if (healthCenterId != "")
                        institution.HealthCenterId = healthCenterId;

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
                        return (0, "Error updating institution", default(USD));
                    }

                    return (1, "Institution updated successfully.", updatedInstitutionDTO);
                }

                return (0, "Institution not found", default(USD));
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        //USD refers to the Usage DTO of an institution
        public async Task<(int, string, USD?)> UploadLicense<USD>(string institutionId, IFormFile? image)
        {
            var filter = Builders<T>.Filter.Eq(I => I.Id, institutionId);
            try
            {
                string? licensePath = null;
                if (image != null)
                {
                    var (fileStatus, fileMessage, filePath) = await _fileService.UploadFile(image, institutionId, "Licenses");
                    if (fileStatus == 1 || filePath == null)
                        return (fileStatus, fileMessage, default(USD));

                    licensePath = filePath;
                }
                var (institutionStatus, institutionMessage, institution) = await GetInstitution(institutionId);

                if (institutionStatus == 0 || institution == null)
                    return (institutionStatus, institutionMessage ?? "Institution doesn't Exist", default(USD));

                institution.LicensePath = licensePath ?? string.Empty;

                var options = new FindOneAndReplaceOptions<T>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var rawInstitution = await _collection.FindOneAndReplaceAsync(filter, institution, options);

                USD updatedInstitution = _mapper.Map<USD>(rawInstitution);
                return (1, "License information Uploaded Successfully", updatedInstitution);
            }
            catch (Exception ex)
            {
                return (1, ex.Message, default(USD));
            }
        }

        //USD refers to the Usage DTO of an institution
        public async Task<(int, string, USD?)> UpdateInstitutionVerification<USD>(string institutionId, bool status)
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
                    return (1, $"Institution Verification set to {status}", _mapper.Map<USD>(result));
                else
                    return (0, "Institution not found", default(USD));
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }
    }

}