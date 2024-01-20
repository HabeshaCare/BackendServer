using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAuthentication.DTOs.ScheduleDTOs;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Utils
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<User, UserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UsageUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UpdateUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, RegistrationDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, LoginDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Doctor, UpdateDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Doctor, UsageDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Doctor, ScheduledDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Schedule, ScheduleDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Schedule, CreateScheduleDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, SchedulerUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}