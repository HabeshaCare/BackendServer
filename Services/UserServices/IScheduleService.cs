using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.DTOs.ScheduleDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IScheduleService
    {
        Task<(int, string, ScheduleDTO?)> GetScheduleById(string scheduleId);
        Task<(int, string, ScheduleDTO[])> GetSchedules(string userId, bool scheduler, int page, int size);
        Task<(int, string, ScheduleDTO?)> CreateSchedule(CreateScheduleDTO createScheduleDTO, string schedulerId);
        Task<(int, string, ScheduleDTO?)> UpdateSchedule(DateTime dateTime, string scheduleId);
        Task<(int, string, ScheduleDTO?)> UpdateScheduleStatus(string scheduleId, bool scheduleStatus);
        Task<(int, string)> DeleteSchedule(string scheduleId);
    }
}