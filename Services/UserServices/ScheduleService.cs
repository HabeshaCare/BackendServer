using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserAuthentication.DTOs.ScheduleDTOs;
using UserAuthentication.Models;
using UserAuthentication.Utils;

namespace UserAuthentication.Services.UserServices
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

        private async Task<(int, string?, Schedule)> GetSchedule(string id)
        {
            try
            {
                var schedule = (await _scheduleCollection.Find(s => s.Id == id).ToListAsync())[0];
                return (1, null, schedule);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
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

        public async Task<(int, string, ScheduleDTO?)> CreateSchedule(CreateScheduleDTO createScheduleDTO, string schedulerId)
        {
            try
            {
                Schedule schedule = _mapper.Map<Schedule>(createScheduleDTO);
                schedule.SchedulerId = schedulerId;

                await _scheduleCollection.InsertOneAsync(schedule);

                ScheduleDTO createdSchedule = await FetchScheduleInformation(schedule, true);

                return (1, "Schedule created successfully", createdSchedule);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, ScheduleDTO?)> GetScheduleById(string scheduleId)
        {
            try
            {
                var (status, message, schedule) = await GetSchedule(scheduleId);
                if (status == 0 || schedule == null)
                    return (0, "Schedule not found", null);

                var scheduleDTO = _mapper.Map<ScheduleDTO>(schedule);
                return (1, "Schedule Found", scheduleDTO);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, ScheduleDTO[])> GetSchedules(string userId, bool scheduler, int page, int size)
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
                    return (0, "No Schedules Found", Array.Empty<ScheduleDTO>());

                ScheduleDTO[] schedules = _mapper.Map<ScheduleDTO[]>(foundSchedules);

                var scheduleTasks = foundSchedules.Select(schedule => FetchScheduleInformation(schedule, scheduler)).ToList();
                schedules = await Task.WhenAll(scheduleTasks);

                return (1, $"Found {schedules.Length}", schedules);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, ScheduleDTO?)> UpdateSchedule(DateTime dateTime, string scheduleId, bool scheduler)
        {
            var filter = Builders<Schedule>.Filter.Eq(u => u.Id, scheduleId);
            var options = new FindOneAndReplaceOptions<Schedule>
            {
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                var (status, message, schedule) = await GetSchedule(scheduleId);

                if (status == 0 || schedule == null)
                    return (0, "Schedule not found", null);

                schedule.ScheduleTime = dateTime;

                var updateSchedule = Task.Run(() => _scheduleCollection.FindOneAndReplaceAsync(filter, schedule, options));
                var fetchScheduleInfo = Task.Run(() => FetchScheduleInformation(schedule, scheduler));
                await Task.WhenAll(updateSchedule, fetchScheduleInfo);

                return (1, "Schedule Updated", fetchScheduleInfo.Result);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, ScheduleDTO?)> UpdateScheduleStatus(string scheduleId, bool scheduleStatus)
        {
            var filter = Builders<Schedule>.Filter.Eq(u => u.Id, scheduleId);
            var options = new FindOneAndReplaceOptions<Schedule>
            {
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                var (status, message, schedule) = await GetSchedule(scheduleId);

                if (status == 0 || schedule == null)
                    return (0, "Schedule not found", null);

                schedule.Confirmed = scheduleStatus;

                var updateSchedule = Task.Run(() => _scheduleCollection.FindOneAndReplaceAsync(filter, schedule, options));
                var fetchScheduleInfo = Task.Run(() => FetchScheduleInformation(schedule, false));
                await Task.WhenAll(updateSchedule, fetchScheduleInfo);

                return (1, "Schedule Updated", fetchScheduleInfo.Result);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string)> DeleteSchedule(string scheduleId)
        {
            try
            {
                var result = await _scheduleCollection.FindOneAndDeleteAsync(s => s.Id == scheduleId);

                if (result == null)
                    return (0, "Schedule not found");
                return (1, "Schedule deleted successfully");
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }
    }
}
