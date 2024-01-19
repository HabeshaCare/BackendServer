using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
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
        
        public ScheduleService(IOptions<MongoDBSettings> options, IMapper mapper):base(options)
        {
            _scheduleCollection = GetCollection<Schedule>("Schedule");
            _usersCollection = GetCollection<User>("Users");
            _doctorCollection = GetCollection<Doctor>("Users");
            _mapper = mapper;
        }

        private async Task<ScheduleDTO> FetchScheduleInformation(Schedule schedule, bool scheduler)
        {
            SchedulerUserDTO schedulerUserInfo;
            ScheduledDoctorDTO doctorInfo;
            var updatedSchedule = _mapper.Map<ScheduleDTO>(schedule);

            if(scheduler)
            {
             var result = await _doctorCollection.Find(d => d.Id == schedule.DoctorId).ToListAsync();
             if(result.Count !=0)
             {
                var doctor = result[0];
                doctorInfo = _mapper.Map<ScheduledDoctorDTO>(doctor);
                updatedSchedule.Doctor = doctorInfo;
             }
            }
            else
            {
                var result = await _usersCollection.Find(u => u.Id == schedule.SchedulerId).ToListAsync();
                if(result.Count !=0)
                {
                    var user = result[0];
                    schedulerUserInfo = _mapper.Map<SchedulerUserDTO>(user);
                    updatedSchedule.Scheduler = schedulerUserInfo;
                }                
            }

            return updatedSchedule;
        }

        public async Task<(int, string, ScheduleDTO?)> GetSchedule(string scheduleId)
        {
            try
            {
                var results = await _scheduleCollection.Find(s => s.Id == scheduleId).ToListAsync();
                if(results.Count == 0)
                    return (0, "Schedule not found", null);
                
                var schedules = _mapper.Map<ScheduleDTO[]>(results);
                return (1, "Schedule Found", schedules[0]);
            }
            catch(Exception ex)
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
                filterDefinition &= filterBuilder.Gte("SchedulerId", userId);
            else
                filterDefinition &= filterBuilder.Lte("DoctorId", userId);

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

        public Task<(int, string, ScheduleDTO?)> UpdateSchedule(DateTime dateTime, string scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<(int, string, ScheduleDTO?)> UpdateScheduleStatus(string scheduleId, bool status)
        {
            throw new NotImplementedException();
        }
        
        public Task<(int, string, ScheduleDTO?)> DeleteSchedule(string scheduleId)
        {
            throw new NotImplementedException();
        }        
    }
}