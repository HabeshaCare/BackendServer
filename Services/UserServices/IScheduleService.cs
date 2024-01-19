using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.DTOs.ScheduleDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IScheduleService
    {
        Task<(int, string, ScheduleDTO?)> GetSchedule(string scheduleId);
        Task<(int, string, ScheduleDTO[])> GetSchedules(string userId, bool scheduler, int page, int size);
        Task<(int, string, ScheduleDTO?)> UpdateSchedule(DateTime dateTime, string scheduleId);
        Task<(int, string, ScheduleDTO?)> UpdateScheduleStatus(string scheduleId, bool status);
        Task<(int, string, ScheduleDTO?)> DeleteSchedule(string scheduleId);
    }
}