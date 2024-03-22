using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.ScheduleDTOs;
using UserManagement.Models;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class ScheduleService : MongoDBService, IScheduleService
    {
        IMongoCollection<Schedule> _scheduleCollection;
        IMongoCollection<User> _usersCollection;
        IMongoCollection<Doctor> _doctorCollection;
        IMapper _mapper;

        public ScheduleService(IOptions<MongoDBSettings> options, IMapper mapper) : base(options)
        {
            _scheduleCollection = GetCollection<Schedule>("Schedule");
            _usersCollection = GetCollection<User>("Users");
            _doctorCollection = GetCollection<Doctor>("Users");
            _mapper = mapper;
        }

        private async Task<SResponseDTO<Schedule>> GetSchedule(string id)
        {
            try
            {
                var schedule = (await _scheduleCollection.Find(s => s.Id == id).ToListAsync())[0];
                return new() { StatusCode = 200, Data = schedule, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message }, Success = false };
            }
        }

        /// Fetches additional schedule information, such as scheduler or doctor details
        private async Task<ScheduleDTO> FetchScheduleInformation(Schedule schedule, bool scheduler)
        {
            SchedulerUserDTO schedulerUserInfo;
            ScheduledDoctorDTO doctorInfo;
            var updatedSchedule = _mapper.Map<ScheduleDTO>(schedule);

            if (scheduler)
            {
                var result = await _doctorCollection.Find(d => d.Id == schedule.DoctorId).ToListAsync();
                if (result.Count != 0)
                {
                    var doctor = result[0];
                    doctorInfo = _mapper.Map<ScheduledDoctorDTO>(doctor);
                    updatedSchedule.Doctor = doctorInfo;
                }
            }
            else
            {
                var result = await _usersCollection.Find(u => u.Id == schedule.SchedulerId).ToListAsync();
                if (result.Count != 0)
                {
                    var user = result[0];
                    schedulerUserInfo = _mapper.Map<SchedulerUserDTO>(user);
                    updatedSchedule.Scheduler = schedulerUserInfo;
                }
            }

            return updatedSchedule;
        }

        public async Task<SResponseDTO<ScheduleDTO>> CreateSchedule(CreateScheduleDTO createScheduleDTO, string schedulerId)
        {
            try
            {
                Schedule schedule = _mapper.Map<Schedule>(createScheduleDTO);
                schedule.SchedulerId = schedulerId;

                await _scheduleCollection.InsertOneAsync(schedule);

                ScheduleDTO createdSchedule = await FetchScheduleInformation(schedule, true);

                return new() { StatusCode = 201, Message = "Schedule created successfully", Data = createdSchedule, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<ScheduleDTO>> GetScheduleById(string scheduleId)
        {
            try
            {
                var response = await GetSchedule(scheduleId);
                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                Schedule schedule = response.Data!;
                var scheduleDTO = _mapper.Map<ScheduleDTO>(schedule);
                return new() { StatusCode = 200, Message = "Schedule Found", Data = scheduleDTO, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<ScheduleDTO[]>> GetSchedules(string userId, bool scheduler, int page, int size)
        {
            var filterBuilder = Builders<Schedule>.Filter;
            var filterDefinition = filterBuilder.Empty;

            int skip = (page - 1) * size;

            if (scheduler)
                filterDefinition &= filterBuilder.Eq("SchedulerId", userId);
            else
                filterDefinition &= filterBuilder.Eq("DoctorId", userId);

            try
            {
                var foundSchedules = await _scheduleCollection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundSchedules.Count == 0)
                    return new() { StatusCode = 404, Errors = new[] { "No Schedules Found" } };

                ScheduleDTO[] schedules = _mapper.Map<ScheduleDTO[]>(foundSchedules);

                var scheduleTasks = foundSchedules.Select(schedule => FetchScheduleInformation(schedule, scheduler)).ToList();
                schedules = await Task.WhenAll(scheduleTasks);
                return new() { StatusCode = 200, Message = $"Found {schedules.Length} schedules", Data = schedules, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<ScheduleDTO>> UpdateSchedule(DateTime dateTime, string scheduleId, bool scheduler)
        {
            var filter = Builders<Schedule>.Filter.Eq(u => u.Id, scheduleId);
            var options = new FindOneAndReplaceOptions<Schedule>
            {
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                var response = await GetSchedule(scheduleId);

                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };
                Schedule schedule = response.Data!;
                schedule.ScheduleTime = dateTime;

                var updateSchedule = Task.Run(() => _scheduleCollection.FindOneAndReplaceAsync(filter, schedule, options));
                var fetchScheduleInfo = Task.Run(() => FetchScheduleInformation(schedule, scheduler));
                await Task.WhenAll(updateSchedule, fetchScheduleInfo);

                return new() { StatusCode = 201, Message = "Schedule Updated", Data = fetchScheduleInfo.Result, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<ScheduleDTO>> UpdateScheduleStatus(string scheduleId, bool scheduleStatus)
        {
            var filter = Builders<Schedule>.Filter.Eq(u => u.Id, scheduleId);
            var options = new FindOneAndReplaceOptions<Schedule>
            {
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                var response = await GetSchedule(scheduleId);

                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                Schedule schedule = response.Data!;
                schedule.Confirmed = scheduleStatus;

                var updateSchedule = Task.Run(() => _scheduleCollection.FindOneAndReplaceAsync(filter, schedule, options));
                var fetchScheduleInfo = Task.Run(() => FetchScheduleInformation(schedule, false));
                await Task.WhenAll(updateSchedule, fetchScheduleInfo);

                return new() { StatusCode = 201, Message = "Schedule Updated", Data = fetchScheduleInfo.Result, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<string>> DeleteSchedule(string scheduleId)
        {
            try
            {
                var result = await _scheduleCollection.FindOneAndDeleteAsync(s => s.Id == scheduleId);

                if (result == null)
                    return new() { StatusCode = 404, Errors = new[] { "Schedule not found" } };

                return new() { StatusCode = 200, Message = "Schedule deleted successfully" };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }
    }
}
