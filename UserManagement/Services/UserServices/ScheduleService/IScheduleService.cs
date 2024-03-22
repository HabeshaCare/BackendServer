using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;
using UserManagement.DTOs.ScheduleDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IScheduleService
    {
        Task<SResponseDTO<ScheduleDTO>> GetScheduleById(string scheduleId);
        Task<SResponseDTO<ScheduleDTO[]>> GetSchedules(string userId, bool scheduler, int page, int size);
        Task<SResponseDTO<ScheduleDTO>> CreateSchedule(CreateScheduleDTO createScheduleDTO, string schedulerId);
        Task<SResponseDTO<ScheduleDTO>> UpdateSchedule(DateTime dateTime, string scheduleId, bool scheduler);
        Task<SResponseDTO<ScheduleDTO>> UpdateScheduleStatus(string scheduleId, bool scheduleStatus);
        Task<SResponseDTO<string>> DeleteSchedule(string scheduleId);
    }
}